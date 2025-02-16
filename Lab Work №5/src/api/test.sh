#!/bin/bash
if [ $(curl -XGET http://example-api:8080/teos -H "Authorization: User user" -f) ]; then
	exit 1
fi

if ! [ $(curl -XGET http://example-api:8080/teos -H "Authorization: User test" -f) ]; then
	exit 1
fi

exit 0