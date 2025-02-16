#!/bin/bash
if [curl -XGET http://localhost/teos -H "Authorization: User test" -f];
	exit 1
fi