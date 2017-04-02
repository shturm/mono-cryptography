# Create self signed certificate
openssl req -x509 -sha256 -nodes -days 365 -newkey rsa:2048 -keyout openssl-generated-pair/privateKey.key -out openssl-generated-pair/certificate.crt

# Combine public and private key in single PFX file. PFX is binary containing both, but can also contain other information like root certificates and certificate chain
openssl pkcs12 -export -out certificate.pfx -inkey openssl-generated-pair/privateKey.key -in openssl-generated-pair/certificate.crt
