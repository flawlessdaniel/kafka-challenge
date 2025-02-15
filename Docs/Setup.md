## Aspectos pendientes:

1. Database connection: tuve un problema inicialmente para correr PostgreSQL con el usuario y password por defecto, asi que es necesario
crear un usuario y contraseña nuevo, por el momento las cadenas de conexion estan configuradas con el siguiente usuario: challenge_user y password: 123456.
2. Secrets: Tenia en mente utilizar algun servicio como Vault o al menos dotnet-usersecrets pero ya no tengo mas tiempo para agregarlo.
3. Tengo algo de codigo repetido entre los servicios, lo cual esta considerado en base al diseño que hice, pero hubiera preferido refactorizarlo un poco tal vez en bibliotecas compartidas.
4. Infraestructura Kafka: Tengo configurado los producers para que creen automaticamente los topics, hubiera sido mejor configurarlo en docker-compose y separar la infraestructura del codigo, pero ya no me dio oportunidad, ademas de que no soy muy expero en Kafka y me hubiera tomado un poco mas de tiempo hacerlo.
5. Tambien tengo algunos valores en codigo duro, como el servidor Redis al registrarlo en los servicios y probablemente algunos mas por alli, que me falto refactorizarlos y usar constantes en su lugar.

## Ejecutar los servicios:

1. Ejecutar docker compose para levantar los servicios con el comando: docker-compose -f ./docker-compose-yaml up -d
2. Crear el usuario, challenge_user, explicado en la seccion de aspectos pendientes (item numero 1). O, en todo caso, actualizar las cadenas de conexion en los archivos appsettings.json en todos los servicios.
3. Ejecutar los servicios con los comandos:
dotnet run -p .\transactionsApi\ o navega hasta la carpeta transactionsApi y ejecuta dotnet run
dotnet run -p .\fraudService\ o navega hasta la carpeta fraudService y ejecuta dotnet run
dotnet run -p .\statusUpdatedService\ o navega hasta la carpeta statusUpdatedService y ejecuta dotnet run
4. Ejecutar los archivos http que estan ubicados en la carpeta Http del proyecto, unicamente verificar en que puerto se ejecuta la API y actualizar los puertos en el archivo http, o utilizar cualquier cliente http de preferencia para probar los endpoints de la API.