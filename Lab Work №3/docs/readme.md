# Лабораторная работа №3
**Тема**: Использование принципов проектирования на уровне методов и классов

**Цель работы**: Получить опыт проектирования и реализации модулей с использованием принципов KISS, YAGNI, DRY, SOLID и др.

# Диаграмма контейнеров:

![Container](https://github.com/user-attachments/assets/e5705cf4-afbc-4e92-8f59-b6bdf92970da)

# Диаграмма компонентов контейнера "WebAPP":

![Component-WebAPP](https://github.com/user-attachments/assets/1511943d-a552-4b36-8deb-5bbe80653445)

# Диаграмма компонентов контейнера "Расчётные блоки":

![Component-Расчётные блоки](https://github.com/user-attachments/assets/60817d8b-7ab5-4353-9157-aa609b62cee8)

# Диаграмма последовательностей
Для построение диаграммы последовательностей варианта использования возьмём вариант использования "Редактирование значений модели":

![Редактирование значений модели](https://github.com/user-attachments/assets/d1697f4c-b3e0-4455-81ed-802fe7720f32)

Диаграмма отражает процесс открытия пользователем страницы с редактируемой таблицей, редактирования значений в таблице и сохранения таблицы.

# Модель БД
На диаграмме отражена упрощённая модель БД.
В полной модели БД группа из 3 таблиц (иерархия значений, помесячные значения модели, параметры модели) повторяется для каждого модуля (17 модулей),
сущности ТЭО и Здание в адресной программе обладают большим количеством свойств и имеют дополнительные словари для некоторых свойств,
а также для некоторых из модулей имеются таблицы с нормативными значениями или дополнительные расчётные таблицы.

![БД](https://github.com/user-attachments/assets/b237402c-9b90-4a81-a1a6-042e1eda8294)

# Применение основных принципов разработки

KISS - keep it simple stupid.
В моём понимании, принцип проявляется в реализации функциональных требований наиболее простым из возможных способов, без создания лишних абстракций ради самих абстракций, а не ради достижения реализации требований.

Пример принципа KISS можно найти в коде загрузки данных для таблицы. За счёт инверсии зависимостей и предобработки данных код лишь выполняет итерацию по всем строка таблицы и вызывает заранее сформированные SQL-запросы, чтобы получить данные.

```python
from calculator.system import vmResource

teo_meta = execution_context.get_input_by_id("teo_meta").value
db_connection = execution_context.get_input_by_id("db_connection").value
row_defs = execution_context.get_input_by_id("defs").value

teo_meta = {
    "teo_id": int(teo_meta["teo_id"]) if teo_meta and "teo_id" in teo_meta.keys() and teo_meta["teo_id"] is not None else 1,
    "teo_step_id": int(teo_meta["teo_step_id"]) if teo_meta and "teo_step_id" in teo_meta.keys() and teo_meta["teo_step_id"] else 1,
    "data_origin_id": int(teo_meta["data_origin_id"]) if teo_meta and "data_origin_id" in teo_meta.keys() and teo_meta["data_origin_id"] else 1,
    "timestamp": int(teo_meta["timestamp"]) if teo_meta and "timestamp" in teo_meta.keys() and teo_meta["timestamp"] else 1
}

output = {
    "defs": row_defs,
    "dates": [],
    "teo_meta": teo_meta
}

for row_def in row_defs:
    if "format" not in row_def:
        row_def["format"] = {}
    if "type" not in row_def["format"]:
        row_def["format"]["type"] = "numeric"

    execution_context.log.info(f"Running query '{row_def['sql']['get']}' with params {(row_def['sql'] | teo_meta)}.")
    data = vmResource.execute(db_connection, row_def["sql"]["get"], row_def["sql"] | teo_meta)
    execution_context.log.info(f"Query yielded {len(data)} rows.")
    if len(output["dates"]) == 0: #Загрузка первой строки
        row_def["values"] = []
        for row in data:
            output["dates"].append(row[0])
            row_def["values"].append(row[1])
    elif len(data) != len(output["dates"]):
        raise Error(f"Количество значений для группы '{row_def['sql']['group']}' не совпадает с количеством значений в предыдущих группах.")
    else:
        row_def["values"] = [0 for x in range(len(output["dates"]))]
        for row in data:
            date, value = row
            try:
                index = output["dates"].index(date)
                row_def["values"][index] = value
            except ValueError:
                raise Error(f"Дата {date} из группы '{row_def['sql']['group']}' отсутствует в предыдущих группах.")


execution_context.add_output("data", output)
```


YAGNI - you aren't gonna need it.
В моём понимании, принцип проявляется в реализации функциональных требований без создания кода/компонентов/абстракций, которые не требуются для реализации текущих требований, как "задел на будущее".
Хотя в целом принцип похож на KISS, этот принцип больше не о простоте кода, а об избежании затрат усилий на "мёртвый" код, которые никогда может не пригодится.

Пример принципа YAGNI можно найти в коде визуализатора таблицы. В таблице реализован ввод, отображение и редактирование только десятичных дробей и процентов, т.к. только они требуются с учётом текущих требований проекта.

```javascript
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
```

DRY - don't repeat yourself.
В моём понимании, принцип проявляется в превращении повторяющегося кода, не имеющего отличий или имеющего минимальные отличия, в переиспользуемые компоненты (функции или классы).

Принцип DRY используется, например, в SQL-запросах.
Во многих случаях требуется получить id записи из таблицы "Иерархия значений" по названию (атрибут name).
При этом атрибут не является уникальным во всей таблицы, но является уникальным в составе (parent_group_id, name).
Таким образом для получения корректного id требуется "спустится" по уровням, начиная с верхнего уровня иерархии (parent_group_id = NULL).
Во избежании повторения запросов вида `SELECT id FROM table_name WHERE parent_group_id = (SELECT id FROM table name WHERE parent_group_id = (...))` реализована функция,
получающая id записи из таблицы "иерархия уровней" по строке, содержащей последовательность названий записей из таблицы "Иерархия значений", формирующих иерархию.

```PLpgSQL
CREATE OR REPLACE FUNCTION teo_calcs.utils_get_group_id(table_name text, group_hierarchy text, delim text default '|')
RETURNS integer
LANGUAGE plpgsql
STABLE
AS $function$
DECLARE
    group_id integer;
    group_name text;
    group_hierarchy_arr text[];
BEGIN
    group_hierarchy_arr = string_to_array(group_hierarchy, delim);
    EXECUTE format(
        'WITH RECURSIVE g(group_id, next_level, group_name) AS ( '
            'SELECT dict.group_id, 2, dict.name FROM teo_dicts.%1$I as dict WHERE name = $1[1] AND parent_group_id IS NULL'
            'UNION ALL '
            'SELECT dict.group_id, g.next_level+1, dict.name '
            'FROM teo_dicts.%1$I as dict INNER JOIN g ON dict.parent_group_id = g.group_id '
            'WHERE dict.name = $1[g.next_level] AND g.next_level <= $2'
        ') '
        'SELECT g.group_id, g.group_name FROM g '
        'ORDER BY g.next_level DESC '
        'LIMIT 1',
        table_name
    )
    INTO group_id, group_name
    USING group_hierarchy_arr, array_length(group_hierarchy_arr, 1);
    IF group_name <> group_hierarchy_arr[array_length(group_hierarchy_arr, 1)] THEN
        RAISE 'Group name mismatch (expected: %, got: %, hierarchy: %). The group doesnt exist?', group_hierarchy_arr[array_length(group_hierarchy_arr, 1)], group_name, group_hierarchy;
    END IF;
    RETURN group_id;
END;
$function$;
```

SOLID. Принцип направлен на объекто-ориентированное программирование, хотя некоторые его части можно применять и в контексте функционального программирования.

S - Single-responsibility principle. У одного класса одна причина для изменения.

Применение принципа SRP невозможно показать на примере проекта, т.к. используется функциональный, а не объектно-ориентированных подход.

0 - Open-closed principle. Классы открыты к расширению, но закрыты к модификации.

Применение принципа OCP невозможно показать на примере проекта, т.к. используется функциональный, а не объектно-ориентированных подход.

L - liskov substitution principle. Дочерние классы должны использоваться так же, как и их базовые (родительские) классы.

Применение принципа LSP невозможно показать на примере проекта, т.к. используется функциональный, а не объектно-ориентированных подход.

I - interface-segregation principle. Интерфейсы должны быть минималистичными, включать только те функции, которые релевантны для пользователей интерфейса.

Применение принципа ISP невозможно показать на примере проекта, т.к. используется функциональный, а не объектно-ориентированных подход.

D - dependency inversion principle. Инверсия зависимостей - код должен ссылаться на на конкретные реализации, а принимать абстракции, реализующие требуемую функциональность.

Принцип инверсии зависимостей можно найти на примере кода загрузки данных таблицы. Код не полагается на прописанный в нём SQL-запрос для получения данных таблицы, а использует передаваемый ему запрос.
```python
from calculator.system import vmResource

teo_meta = execution_context.get_input_by_id("teo_meta").value
db_connection = execution_context.get_input_by_id("db_connection").value
row_defs = execution_context.get_input_by_id("defs").value

teo_meta = {
    "teo_id": int(teo_meta["teo_id"]) if teo_meta and "teo_id" in teo_meta.keys() and teo_meta["teo_id"] is not None else 1,
    "teo_step_id": int(teo_meta["teo_step_id"]) if teo_meta and "teo_step_id" in teo_meta.keys() and teo_meta["teo_step_id"] else 1,
    "data_origin_id": int(teo_meta["data_origin_id"]) if teo_meta and "data_origin_id" in teo_meta.keys() and teo_meta["data_origin_id"] else 1,
    "timestamp": int(teo_meta["timestamp"]) if teo_meta and "timestamp" in teo_meta.keys() and teo_meta["timestamp"] else 1
}

output = {
    "defs": row_defs,
    "dates": [],
    "teo_meta": teo_meta
}

for row_def in row_defs:
    if "format" not in row_def:
        row_def["format"] = {}
    if "type" not in row_def["format"]:
        row_def["format"]["type"] = "numeric"

    execution_context.log.info(f"Running query '{row_def['sql']['get']}' with params {(row_def['sql'] | teo_meta)}.")
    data = vmResource.execute(db_connection, row_def["sql"]["get"], row_def["sql"] | teo_meta)
    execution_context.log.info(f"Query yielded {len(data)} rows.")
    if len(output["dates"]) == 0: #Загрузка первой строки
        row_def["values"] = []
        for row in data:
            output["dates"].append(row[0])
            row_def["values"].append(row[1])
    elif len(data) != len(output["dates"]):
        raise Error(f"Количество значений для группы '{row_def['sql']['group']}' не совпадает с количеством значений в предыдущих группах.")
    else:
        row_def["values"] = [0 for x in range(len(output["dates"]))]
        for row in data:
            date, value = row
            try:
                index = output["dates"].index(date)
                row_def["values"][index] = value
            except ValueError:
                raise Error(f"Дата {date} из группы '{row_def['sql']['group']}' отсутствует в предыдущих группах.")


execution_context.add_output("data", output)
```

# Дополнительные принципы разработки

BDUF - Big design up front.
В моём понимании, смысл данного принципа заключается в полном препланировании архитектуры системы перед её реализацией и неизменности созданной архитектуры в процессе разработки.

Данный принцип не подходит реализуемой системе, так как методика формирования ТЭО в холдинге "ЭР-Телеком" относительно часто изменяется с разной степенью серьёзности изменений, что неизбежно приведёт к несоответствии препланированной архитектуры требованиям.

SoC - Separation of concerns.
В моём понимании, смысл данного принципа заключается в том, что один компонент выполняет одну конкретную задачу и скрывает всю информации о задаче, которая не требуется другим компонентам для выполнения их задач.

В контексте реализуемой архитектуры системы (low-code платформа с блоками кодам) данный принцип хорошо подходит для реализуемой архитектуры, т.к. имеет смысл передавать минимальное количество информации между блоками для ускорения работы системы.

MVP - minimum viable product.
В моём понимании, смысл данного принципа заключается в реализации в первой итерации продукта, который будет отвечать минимальному набору функциональных требований,
чтобы быть способным приносить какую-то практическую пользу, даже если он не будет обладать наилучшим UX и отвечать всем требованиям.
Целью разработки такого продукта является получение обратной связи от пользователя на раннем этапе, чтобы не тратить усилия на реализацию, которая окажется ненужно пользователю.

Данный принцип хорошо подходит реализуемой системе, т.к. конечные требования заказчика всё-ещё остаются расплывчатыми и по ходу разработки появляются новые требования.

PoC - proof of concept.
В моём понимании, смысл данного принципа заключается в реализации демонстрационного примера системы, который показывает, что реализация полноразмерной системы возможна и имеет экономический смысл.

В отношении данной системы этот принцип не применим по двум причинам. Во-первых, система не подразумевает технически сложную реализацию. Во-вторых, в контексте общих требований системы невозможно создать доказательство концепции, которое покажет экономическую осмысленность разработки системы.
