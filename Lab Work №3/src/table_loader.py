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