DECLARE @ConversationHandle UNIQUEIDENTIFIER;
DECLARE @MessageTypeName NVARCHAR(256);
DECLARE @MessageBody XML;
DECLARE @ServiceName NVARCHAR(256);
DECLARE @MessageContract NVARCHAR(256);

WAITFOR (
    RECEIVE TOP(1)
        @ConversationHandle = conversation_handle,
        @MessageTypeName = message_type_name,
        @MessageBody = CAST(message_body AS XML),
        @ServiceName = service_name,
        @MessageContract = service_contract_name
        FROM dbo.SomeServiceQueue
), TIMEOUT 3000;
