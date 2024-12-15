const block = this;
const root = this.Element.nativeElement;

root.innerHTML = `
    <button class="teo-btn">
        Рассчитать по методологии
    </button>
    <div id="tablecontainer" style="margin-top:10px;width:100%;height:fit-content">

    </div>
`;

const input_type_handlers = {
    numeric: {
        display_format: function(value) {
            return value;
        },
        storage_format: function(value) {
            value = +(value ?? undefined);
            if (isNaN(value))
                value = null;
            return value;
        },
        create_input: function(params) {
            var input = document.createElement("input");
            input.type = "number";
            input.onpaste = (event) => {
                let content = (event.clipboardData || window.clipboardData).getData("text");
                if (isNaN(+content))
                    event.preventDefault();
            };
            input.min = params.min;
            input.max = params.max;
            return input;
        }
    },
    percentage: {
        display_format: function(value) {
            return value*100;
        },
        storage_format: function(value) {
            value = +(value ?? undefined);
            if (isNaN(value))
                value = null;
            else
                value /= 100;
            return value;
        },
        create_input: function(params) {
            var input = document.createElement("input");
            input.type = "number";
            input.onpaste = (event) => {
                let content = (event.clipboardData || window.clipboardData).getData("text");
                if (isNaN(+content))
                    event.preventDefault();
            };
            input.min = params.min;
            input.max = params.max;
            return input;
        }
    }
}

const tableContainer = root.querySelector("#tablecontainer");

root.style.width = "100%";
root.style.height = "100%";

const input_data = {{ input_data | tojson }};

//Генерируем полу-случайный идентификатор для таблицы
const table_uuid = input_data.defs.map(x => x.sql.group).join("#") + "#" + Math.random().toString(16).slice(2);
//Управляем кнопкой расчёта по методологии
if (!input_data.defs.some(x => x.autocalc)) {
    root.querySelector(".teo-btn").style.display = "none";
}
else {
    root.querySelector(".teo-btn").onclick = () => {
        Toastify({
            text: "Расчет...",
            duration: 2000,
            style: {
                background: "linear-gradient(to right, #00b09b, #96c93d)"
            }
        }).showToast();
        block.OnPointClickEventHandler({
            type: "autocalc",
            table_uuid: table_uuid,
            teo_meta: input_data.teo_meta
        });
    };
}

const columns = [
    {
        type: "text",
        title: "Продукт",
        width: "300",
        readOnly: true,
        wordWrap: true
    }
]

let table;

function getCellPos(cell) {
    return {
        x: +cell.getAttribute("data-x"),
        y: +cell.getAttribute("data-y")
    };
}

const customColumn = {
    // Methods
    closeEditor : function(cell, save) {
        let value = cell.children[0].value;
        if (value == "")
            value = "-";

        if (!save)
            value = cell.oldValue;

        value = (value ?? "-").toString();
        cell.children[0].type = "text";
        cell.children[0].value = value;
        cell.children[0].onblur = undefined;
        return value;
    },
    openEditor : function(cell) {
        debugger;
        const pos = getCellPos(cell);
        const fmt = input_data.defs[pos.y].format;
        const format_controller = input_type_handlers[fmt.type];
        // Create input
        var element = format_controller.create_input(fmt);
        cell.oldValue = element.value = cell.innerHTML;
        element.onblur = () => table.closeEditor(cell, 1);

        // Update cell
        cell.classList.add('editor');
        cell.innerHTML = '';
        cell.appendChild(element);
        // Focus on the element
        element.focus();
    },
    updateCell: function(cell, value, force) {
        if (value == "")
            value = "-";
        
        value = +value;
        if (isNaN(value))
            value = null;

        value = (value ?? "-").toString();
        cell.innerHTML = value;
        return value;
    },
    getValue : function(cell) {
        return cell.innerHTML;
    },
    setValue : function(cell, value) {
        cell.innerHTML = value;
    }
}

for (const date of input_data.dates) {
    columns.push(
        {
            editor: customColumn,
            title: date,
            width: "120",
            readOnly: false,
            wordWrap: false,
            disableWordWrap: true //Кастомный параметр, смотри блок "Библиотеки". Нужен для этих столбцов.
        }
    );
}

var changes = {};

const toolbar = [];

if (input_data.defs.some(x => !x.readonly))
{
    toolbar.push({
        type: "i",
        content: "save",
        onclick: function () {
            table.resetSelection();
            Toastify({
                text: "Сохранение...",
                duration: 2000,
                style: {
                    background: "linear-gradient(to right, #00b09b, #96c93d)"
                }
            }).showToast();
            block.OnPointClickEventHandler({
                type: "save",
                table_uuid: table_uuid,
                diffs: changes,
                teo_meta: input_data.teo_meta
            });
            changes = {};
        }
    });
}

const table_data = input_data.defs.map(x => [x.name, ...x.values.map(y => y == null ? '-' : input_type_handlers[x.format.type].display_format(y))]);

const table_config = {
    columns: columns,
    data: table_data,
    onbeforechange: function (instance, cell, x, y, value) {
        debugger;
        const def = input_data.defs[y];
        const fmt = def.format;
        const format_controller = input_type_handlers[fmt.type];

        if (value?.length == 0)
            value = null;
        else
            value = format_controller.storage_format(value);

        
        if (changes[def.sql.group] == undefined)
            changes[def.sql.group] = [];
        const index = changes[def.sql.group].findIndex(el => el.date == input_data.dates[x-1]);
        if (index == -1)
            changes[def.sql.group].push({
                date: input_data.dates[x-1],
                value: value
            });
        else
             changes[def.sql.group][index].value = value;
             
        if (value == null)
            value = '-';
        else
            value = format_controller.display_format(value).toFixed(2);
        table_data[y][x] = value;
        return value;
    },
    onbeforepaste: function(instance, value, x, y) {
        x = +x;
        y = +y;
        const cell = table.getCellFromCoords(x, y);
        if(cell.classList.contains("readonly"))
            return false;
        const style = cell.style.cssText;
        setTimeout(() => cell.style.cssText = style);
        return value;
    },
    contextMenu: false,
    toolbar: toolbar,
    columnSorting: false,
    allowInsertRow: false,
    allowInsertColumn: false,
    allowDeleteRow: false,
    allowDeleteColumn: false
};

table = jexcel(tableContainer, table_config);
table.hideIndex();
table.refresh();

for (let y = 0; y < input_data.defs.length; y++)
{
    for (let i = 0; i < input_data.dates.length; i++) {
        if (input_data.defs[y].readonly)
            table.getCellFromCoords(i+1, y).classList.add("readonly");
        else
            table.getCellFromCoords(i+1, y).style.background = "#FFFCBD";
    }
}

for (const col of root.querySelectorAll(".jexcel_selectcolumn"))
    col.style.top = "0px";
for (const col of root.querySelectorAll(".jexcel_selectall"))
    col.style.top = "0px";
for (const col of root.querySelectorAll(".jexcel_index"))
    col.style.top = "0px";

if (document.teo_jexcel_updater == null)
    document.teo_jexcel_updater = {}

document.teo_jexcel_updater[table_uuid] = function(group, date, value) {
    const row = input_data.defs.findIndex(x => x.sql.group == group);
    const column = input_data.dates.indexOf(date);

    if (row == -1 || column == -1)
        return false;

    const change = changes[group]?.findIndex(x => x.date == date);
    if (change != null && change != -1)
        changes[group].splice(change, 1);

    table.ignore_history = true;
    table.ignoreEvents = true;
    input_data.defs[row].values[column] = value;
    table.setValueFromCoords(column+1, row, (value ?? '-').toString(), true);
    table.ignore_history = false;
    table.ignoreEvents = false;
    return true;
}

block.OnPointClickEventHandler({
    type: "first-load"
});