#!/bin/bash
if [ $(curl -XGET http://127.0.0.1:8080/teos -H "Authorization: User user" -f) ]; then
	exit 1
fi

if ! [ $(curl -XGET http://127.0.0.1:8080/teos -H "Authorization: User test" -f) ]; then
	exit 1
fi

exit 0