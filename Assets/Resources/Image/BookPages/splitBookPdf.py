import os
from PyPDF2 import PdfReader, PdfWriter
from PIL import Image
from pdf2image import convert_from_path

def pdf_to_png_with_transparency(input_pdf_path):
    # Create a directory to save the PNG pages
    output_dir = os.path.join(os.path.dirname(input_pdf_path), 'png_pages_with_transparency')
    os.makedirs(output_dir, exist_ok=True)

    # Convert PDF to images (one per page)
    pages = convert_from_path(input_pdf_path)

    # Process each page image
    for i, page in enumerate(pages):
        # Convert to RGBA to add transparency
        page = page.convert("RGBA")
        
        # Make white pixels (255, 255, 255) transparent
        datas = page.getdata()
        new_data = []
        for item in datas:
            threshold = 250
            # Change all white (also consider off-white) pixels to transparent
            if item[0] > threshold and item[1] > threshold and item[2] > threshold:  # Adjust threshold as needed
                new_data.append((255, 255, 255, 0))  # Transparent
            else:
                new_data.append(item)
        
        # Apply new data to the image
        page.putdata(new_data)

        # Save each page as a PNG file
        output_image_path = os.path.join(output_dir, f'page{i+1}.png')
        page.save(output_image_path, 'PNG')

    print(f"PDF conversion to PNG with transparency completed. Pages saved in: {output_dir}")

# Usage example
input_pdf_path = "./4.pdf"  # Replace with your PDF file path
pdf_to_png_with_transparency(input_pdf_path)
