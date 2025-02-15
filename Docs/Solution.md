### Diseño de solucion para el challenge

Revisar el diagrama solution_kafka_transactions.png en donde se encuentra el diseño de la solucion.

## Topics:
    - transactions
    - transactions-retries
    - status-update

## Flujo:

1. Api crea la transaccion en la base de datos como pendiente.
2. Api publica la transaccion en el topic transactions.
3. El servicio de fraude consume las transacciones y usando el AccountId como key, lo marca como "locked" en Redis para asegurar que en caso se reciban nuevas transacciones para la misma cuenta, las transacciones sean procesadas en orden y no se procese una transaccion hasta que una previa sea completamente actualizada en base de datos. De esta manera poder asegurar que el resumen de transacciones diarios pueda respetar la validacion de no exceder los 20mil por dia.
4. En caso el servicio de fraude consuma una transaccion y detecte que la cuenta esta marcada como "locked", envia la transaccion al topic "Retries". En una siguiente iteracion el servicio de fraude evaluara las transacciones en el topic "Retries" y si determina que la cuenta ya no esta en un estado "locked", las reenviara al topic transactions para que se procesen normalmente.
5. Cuando el servicio de fraude procesa la transaccion, y determina el status haciendo una comparacion contra la tabla de TransactionSummary. Posteriormente, envia la transaccion validada al topic status-update.
6. El servicio StatusUpdatedService, se encarga de actualizar las transacciones en la base de datos y de marcar el AccountId como "unlocked" de ser necesario.
7. El concepto de "unlocked", se entiende en el sistema, cuando el key(AccountId) no esta presente en Redis.
8. Si se reciben multiples transacciones para el mismo AccountId, estas se iran acumulando en un contador en Redis. Posteriormente, conforme se vayan procesando, el servicio StatusUpdatedService, ira disminuyendo el contador, hasta llegar a 0 y eliminar el key de Redis.

## Decisiones Principales:

1. Se creo el servicio StatusUpdatedService para poder asignar la responsabilidad de actualizaciones en base de datos y no tener esta responsabilidad en la API ni en el servicio de Fraude, de esta manera, cada servicio puede escalar independientemente.
2. Se uso como key el AccountId, para poder asegurar que cada mensaje correspondiente a una cuenta, sea enviada al mismo partition y sea consumido por el mismo consumidor en caso de escalar los servicios.
3. Se incluyo Redis en el diseño y el topic de Retries para evitar problemas de concurrencia en donde transacciones consecutivas podrias considerarse aprobadas cuando en realidad deberian ser rechazadas.
4. Se crearon 2 tablas: Transactions y TransactionsSummary, para poder desacoplar el proceso de creacion de transacciones unicamente y el proceso de validacion diaria. De esta manera, las consultas para validacion son mas eficientes, ya que solo es necesario hacer un query de lectura a la tabla Summary usando el AccountId y la fecha actual. De esta manera tambien es posible evitar conflictos entre servicios haciendo operaciones de lectura y escritura sobre la misma tabla.