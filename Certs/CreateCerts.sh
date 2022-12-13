#!/bin/bash

#CA Authority
openssl genrsa -out test-ca.private-key.pem 4096
openssl rsa -in test-ca.private-key.pem -pubout -out test-ca.public-key.pem
openssl req -new -x509 -key test-ca.private-key.pem -out test-ca.cert.pem -days 365 -config test-ca.cnf
#openssl pkcs12 -export -inkey test-ca.private-key.pem -in test-ca.cert.pem -out test-ca.pfx -passout file:Password.txt
openssl x509 -in test-ca.cert.pem -out test-ca.crt

#nutritionwebclient
openssl genrsa -out nutritionwebclient.private-key.pem 4096
openssl rsa -in nutritionwebclient.private-key.pem -pubout -out nutritionwebclient.public-key.pem
openssl req -new -sha256 -key nutritionwebclient.private-key.pem -out nutritionwebclient.csr -config nutritionwebclient.cnf
openssl x509 -req -in nutritionwebclient.csr -CA test-ca.cert.pem -CAkey test-ca.private-key.pem -CAcreateserial -out nutritionwebclient.cer -days 365 -sha256 -extfile nutritionwebclient.cnf -extensions req_v3
openssl pkcs12 -export -inkey nutritionwebclient.private-key.pem -in nutritionwebclient.cer -out NutritionWebClient.pfx -passout file:Password.txt

#gateway
openssl genrsa -out ocelotgateway.private-key.pem 4096
openssl rsa -in ocelotgateway.private-key.pem -pubout -out ocelotgateway.public-key.pem
openssl req -new -sha256 -key ocelotgateway.private-key.pem -out ocelotgateway.csr -config ocelotgateway.cnf
openssl x509 -req -in ocelotgateway.csr -CA test-ca.cert.pem -CAkey test-ca.private-key.pem -CAcreateserial -out ocelotgateway.cer -days 365 -sha256 -extfile ocelotgateway.cnf -extensions req_v3
openssl pkcs12 -export -inkey ocelotgateway.private-key.pem -in ocelotgateway.cer -out APIGateway.pfx -passout file:Password.txt

#products
openssl genrsa -out productsapi.private-key.pem 4096
openssl rsa -in productsapi.private-key.pem -pubout -out productsapi.public-key.pem
openssl req -new -sha256 -key productsapi.private-key.pem -out productsapi.csr -config productsapi.cnf
openssl x509 -req -in productsapi.csr -CA test-ca.cert.pem -CAkey test-ca.private-key.pem -CAcreateserial -out productsapi.cer -days 365 -sha256 -extfile productsapi.cnf -extensions req_v3
openssl pkcs12 -export -inkey productsapi.private-key.pem -in productsapi.cer -out ProductsCatalog.pfx -passout file:Password.txt

#usermanagement(userlogin)
openssl genrsa -out usermanagement.private-key.pem 4096
openssl rsa -in usermanagement.private-key.pem -pubout -out usermanagement.public-key.pem
openssl req -new -sha256 -key usermanagement.private-key.pem -out usermanagement.csr -config usermanagement.cnf
openssl x509 -req -in usermanagement.csr -CA test-ca.cert.pem -CAkey test-ca.private-key.pem -CAcreateserial -out usermanagement.cer -days 365 -sha256 -extfile usermanagement.cnf -extensions req_v3
openssl pkcs12 -export -inkey usermanagement.private-key.pem -in usermanagement.cer -out UserManagement.pfx -passout file:Password.txt

#userregister
openssl genrsa -out userregister.private-key.pem 4096
openssl rsa -in userregister.private-key.pem -pubout -out userregister.public-key.pem
openssl req -new -sha256 -key userregister.private-key.pem -out userregister.csr -config userregister.cnf
openssl x509 -req -in userregister.csr -CA test-ca.cert.pem -CAkey test-ca.private-key.pem -CAcreateserial -out userregister.cer -days 365 -sha256 -extfile userregister.cnf -extensions req_v3
openssl pkcs12 -export -inkey userregister.private-key.pem -in userregister.cer -out UserRegister.pfx -passout file:Password.txt

#jwtservice
openssl genrsa -out jwtservice.private-key.pem 4096
openssl rsa -in jwtservice.private-key.pem -pubout -out jwtservice.public-key.pem
openssl req -new -sha256 -key jwtservice.private-key.pem -out jwtservice.csr -config jwtservice.cnf
openssl x509 -req -in jwtservice.csr -CA test-ca.cert.pem -CAkey test-ca.private-key.pem -CAcreateserial -out jwtservice.cer -days 365 -sha256 -extfile jwtservice.cnf -extensions req_v3
openssl pkcs12 -export -inkey jwtservice.private-key.pem -in jwtservice.cer -out JWTService.pfx -passout file:Password.txt

#predefinedmeals
openssl genrsa -out meals.private-key.pem 4096
openssl rsa -in meals.private-key.pem -pubout -out meals.public-key.pem
openssl req -new -sha256 -key meals.private-key.pem -out meals.csr -config predefinedmeals.cnf
openssl x509 -req -in meals.csr -CA test-ca.cert.pem -CAkey test-ca.private-key.pem -CAcreateserial -out meals.cer -days 365 -sha256 -extfile predefinedmeals.cnf -extensions req_v3
openssl pkcs12 -export -inkey meals.private-key.pem -in meals.cer -out PredefinedMeals.pfx -passout file:Password.txt

#myday
openssl genrsa -out myday.private-key.pem 4096
openssl rsa -in myday.private-key.pem -pubout -out myday.public-key.pem
openssl req -new -sha256 -key myday.private-key.pem -out myday.csr -config myday.cnf
openssl x509 -req -in myday.csr -CA test-ca.cert.pem -CAkey test-ca.private-key.pem -CAcreateserial -out myday.cer -days 365 -sha256 -extfile myday.cnf -extensions req_v3
openssl pkcs12 -export -inkey myday.private-key.pem -in myday.cer -out MyDayService.pfx -passout file:Password.txt

#logs
openssl genrsa -out logs.private-key.pem 4096
openssl rsa -in logs.private-key.pem -pubout -out logs.public-key.pem
openssl req -new -sha256 -key logs.private-key.pem -out logs.csr -config logs.cnf
openssl x509 -req -in logs.csr -CA test-ca.cert.pem -CAkey test-ca.private-key.pem -CAcreateserial -out logs.cer -days 365 -sha256 -extfile logs.cnf -extensions req_v3
openssl pkcs12 -export -inkey logs.private-key.pem -in logs.cer -out LogsService.pfx -passout file:Password.txt