import os
import subprocess

html_path = os.path.abspath(r"docs/PROPOSTA COMERCIAL/Documento_Proposta_Comercial.html")
pdf_path = os.path.abspath(r"docs/PROPOSTA COMERCIAL/PROPOSTA COMERCIAL.pdf")

chrome = r"C:\Program Files\Google\Chrome\Application\chrome.exe"
file_url = f"file:///{html_path.replace(os.sep, '/')}"

print(f"Gerando PDF da Proposta Comercial a partir de: {file_url}")
cmd_pdf = [
    chrome,
    "--headless=new",
    "--disable-gpu",
    "--no-pdf-header-footer",
    f"--print-to-pdf={pdf_path}",
    file_url
]

res_pdf = subprocess.run(cmd_pdf, capture_output=True, text=True)
print("Return code PDF:", res_pdf.returncode)
print("Stdout:", res_pdf.stdout)
print("Stderr:", res_pdf.stderr)

if os.path.exists(pdf_path):
    size = os.path.getsize(pdf_path)
    print(f"Sucesso! PDF gerado: {pdf_path} ({size:,} bytes)")
else:
    print("Erro: PDF não foi gerado!")
