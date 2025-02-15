### Transaction statuses
1. pending
2. approved
3. rejected

### The following are the rejection criteria:
- Every transaction with a value greater than 2000.
- Accumulated per day is greater than 20000.

### CREATE TRANSACTION

#### ENDPOINT REQUEST
```json
{
    "sourceAccountId": "Guid",
    "targetAccountId": "Guid",
    "tranferTypeId": 1,
    "value": 120
}
```

#### RESPONSE
```json
{
    "TransactionId": "Guid",
    "CreatedAt": "datetime string dd/MM/yyyy HH:mm:ss"
}
```

#### DATABASE
```json
{
    "Id": "Guid",
    "SourceAccountId": "Guid",
    "TargetAccountId": "Guid",
    "TranferTypeId": "number",
    "Value": "real",
    "Status": "number",
    "CreatedAt": "date and timestamp"
}
```

#### CREATED EVENT - RESULT FROM COMMAND
```json
{
    "TransactionId": "Guid",
    "AccountId": "Guid",
    "Value": "real",
    "CreatedAt": "date and timestamp"
}
```

#### FRAUD CONSUMER - RESULT VALIDATION
```json
{
    "TransactionId": "Guid",
    "AccountId": "Guid",
    "Value": "real",
    "CreatedAt": "date and timestamp",
    "Status": "number"
}
```



### GET TRANSACTION

#### ENDPOINT REQUEST
```json
{
    "transactionExternalId": "Guid",
    "createdAt": "Date"
}
```

#### RESPONSE
```json
{
    "TransactionExternalId": "Guid",
    "sourceAccountId": "Guid",
    "targetAccountId": "Guid",
    "TranferTypeId": "number",
    "Value": "real",
    "Status": "string",
    "CreatedAt": "datetime string dd/MM/yyyy HH:mm:ss"
}
```