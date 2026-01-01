!/bin/bash

# Contar líneas de código C# ignorando bin y obj
total=$(find . -type f -name "*.cs" \
    -not -path "*/bin/*" \
    -not -path "*/obj/*" \
    -exec wc -l {} + | awk '{total += $1} END {print total}')

echo "📊 Total de líneas de código C#: $total"
