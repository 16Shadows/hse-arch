#!/bin/bash
sleep 15

if [ $(curl -XGET http://localhost:8080/teos -H "Authorization: User user" -f) ]; then
	exit 1
fi

if ! [ $(curl -XGET http://localhost:8080/teos -H "Authorization: User test" -f) ]; then
	exit 1
fi

exit 0