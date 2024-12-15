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
            'SELECT dict.group_id, 2, dict.name FROM teo_dicts.%1$I as dict WHERE name = $1[1] '
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