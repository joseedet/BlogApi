#!/bin/bash

# Contar líneas de código C# ignorando:
# - bin/ y obj/
# - líneas en blanco
# - comentarios //, ///, /* */, * dentro de bloques

total=$(find . -type f -name "*.cs" \
    -not -path "*/bin/*" \
    -not -path "*/obj/*" | while read file; do
        awk '
            BEGIN { in_comment=0 }
            {
                line=$0

                # Detectar inicio de comentario multilinea
                if (match(line, /\/\*/)) {
                    in_comment=1
                }

                # Si estamos dentro de comentario multilinea
                if (in_comment==1) {
                    # Detectar fin de comentario multilinea
                    if (match(line, /\*\//)) {
                        in_comment=0
                    }
                    next
                }

                # Ignorar líneas en blanco
                if (line ~ /^[[:space:]]*$/) next

                # Ignorar comentarios de línea
                if (line ~ /^[[:space:]]*\/\//) next

                # Ignorar comentarios XML /// 
                if (line ~ /^[[:space:]]*\/\//) next

                # Si llega aquí, es línea de código
                count++
            }
            END { print count }
        ' "$file"
    done | awk '{total += $1} END {print total}')

echo "📊 Total de líneas de código C# (solo código real): $total"

