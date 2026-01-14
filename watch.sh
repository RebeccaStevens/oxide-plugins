#!/bin/bash

projects=(
    "UiBuilderLibrary"
    "ConfigMaster"
    "ResourceMaster"
)

# Watch for changes and rebuild.
inotifywait \
    -m \
    -r \
    -e modify,create,delete \
    --include "\.cs$" \
    --format '%w %f' \
    src \
| while read -r dir filename; do
    if [[ "$dir" =~ src/([A-Za-z0-9]+) ]]; then
        project="${BASH_REMATCH[1]}"
    fi
    
    if [[ " ${projects[*]} " =~ " ${project} " ]]; then
        plugin.merge -m -c -p "./src/$project/merge.yml"
    fi
done