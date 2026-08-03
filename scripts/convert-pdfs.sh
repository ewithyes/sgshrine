#!/bin/bash
# Converts a folder of PDF issues into zero-padded Chapter subfolders of JPGs,
# ready for the ImportTool.
#
# Usage: ./scripts/convert-pdfs.sh <pdf-folder> <output-folder>

set -e  # stop immediately if any command fails

pdf_folder="$1"
output_folder="$2"

if [ -z "$pdf_folder" ] || [ -z "$output_folder" ]; then
    echo "Usage: ./convert-pdfs.sh <pdf-folder> <output-folder>"
    exit 1
fi

if [ ! -d "$pdf_folder" ]; then
    echo "Error: PDF folder not found at '$pdf_folder'"
    exit 1
fi

mkdir -p "$output_folder"

chapter_num=1
pdf_count=$(find "$pdf_folder" -maxdepth 1 -name "*.pdf" | wc -l | tr -d ' ')

if [ "$pdf_count" -eq 0 ]; then
    echo "No PDFs found in '$pdf_folder'"
    exit 1
fi

echo "Found $pdf_count PDF(s). Converting..."

for pdf in "$pdf_folder"/*.pdf; do
    padded=$(printf "%02d" "$chapter_num")
    chapter_dir="$output_folder/Chapter$padded"
    mkdir -p "$chapter_dir"

    echo "  Chapter$padded  <-  $(basename "$pdf")"
    pdftoppm -jpeg "$pdf" "$chapter_dir/page"

    chapter_num=$((chapter_num + 1))
done

echo "Done. Converted $((chapter_num - 1)) issue(s) into '$output_folder'."