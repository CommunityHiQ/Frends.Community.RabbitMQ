# Frends.Community.RabbitMQ
Frends task for operating on RabbitMQ queues. Supports reading and writing from queue.

- [Installing](#installing)
- [Tasks](#tasks)
  - [Write Message](#writemessage)
  - [Write Message String](#writemessagestring)
  - [Read Message](#readmessage)
  - [Read Message String](#readmessagestring)
- [License](#license)
- [Building](#building)
- [Contributing](#contributing)
- [Change Log](#change-log)

# Installing
You can install the task via FRENDS UI Task View or you can find the nuget package from the following nuget feed
'Nuget feed coming at later date'

Tasks
=====

## WriteMessage

### Task Parameters

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| Data | byte[] | Data to be put in message body| new byte[]{1,2,3}|
| QueueName | string | Name of the queue | sampleQueue |
| ExchangeName | string | Name of the exchange | sampleExchange |
| RoutingKey | string | Routing key (as in RabbitMQ specification) | sampleQueue |
| HostName | string | Address of the server hosting RabbitMQ | localhost or amqp://user:password@hostname:port/vhost |
| WriteMessageCount | string | Amount of messages in the buffer which will be sent over messaging queue as a batch.  | 20 |
| ConnectWithURI | bool | If true, hostname should be an URI | If false, use hostname only |
| Create | bool | True to declare queue before writing | False to not declare it|
| Durable | bool | Set durable option when creating queue |

## WriteMessageString
| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| Data | string | Data to be put in message body| "abc"|
| QueueName | string | Name of the queue | sampleQueue |
| ExchangeName | string | Name of the exchange | sampleQueue |
| RoutingKey | string | Routing key (as in RabbitMQ specification) | sampleQueue |
| HostName | string | Address of the server hosting RabbitMQ | localhost or amqp://user:password@hostname:port/vhost |
| ConnectWithURI | bool | If true, hostname should be an URI | If false, use hostname only |
| Create | bool | True to declare queue before writing | False to not declare it|
| Durable | bool | Set durable option when creating queue |


## ReadMessage

### Task Parameters

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| QueueName | string | Name of the queue | sampleQueue |
| HostName | string | Address of the server hosting RabbitMQ | localhost or amqp://user:password@hostname:port/vhost |
| ReadMessageCount | int | Maximum number of messages to be read from queue. It can exceed number of available messages. | 1 |
| AutoAck | enum |  Set acknowledgement type. AutoAck,AutoNack, AutoNackAndRequeue,AutoReject,AutoRejectAndRequeue,ManualAck| ReadAckType.AutoAck |
| ConnectWithURI | bool | If true, hostname should be an URI | If false, use hostname only |

### Output

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| Messages | List<Message> | A list of message-objects | |

### Message-object 

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| Data | string (base64 encoded byte[]) | | |
| MessageCount | uint | | |
| DeliveryTag | ulong | | |

### Read message sample JSON

{"Messages":[{"Data":"AAEC","MessagesCount":0,"DeliveryTag":1}]}

## ReadMessageString

### Task Parameters

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| QueueName | string | Name of the queue | sampleQueue |
| HostName | string | Address of the server hosting RabbitMQ | localhost or amqp://user:password@hostname:port/vhost |
| ReadMessageCount | int | Maximum number of messages to be read from queue. It can exceed number of available messages. | 1 |
| AutoAck | enum |  Set acknowledgement type. AutoAck,AutoNack, AutoNackAndRequeue,AutoReject,AutoRejectAndRequeue,ManualAck| ReadAckType.AutoAck |
| ConnectWithURI | bool | If true, hostname should be an URI | If false, use hostname only |

### OutputString

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| Messages | List<MessageString> | A list of MessageString-objects | |


### MessageString-object 

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| Data | string (UTF8 converted byte[]) | | |
| MessageCount | uint | | |
| DeliveryTag | ulong | | |




# License

This project is licensed under the MIT License - see the LICENSE file for details

# Building

Clone a copy of the repo

`git clone https://github.com/CommunityHiQ/Frends.Community.RabbitMQ.git`

Restore dependencies

`nuget restore frends.community.rabbitmq`

Rebuild the project

Run Tests with nunit3. Tests can be found under

`Frends.Community.Email.Tests\bin\Release\Frends.Community.RabbitMQ.Tests.dll`

Create a nuget package

`nuget pack nuspec/Frends.Community.RabbitMQ.nuspec`

# Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

# Change Log

| Version             | Changes                 |
| ---------------------| ---------------------|
| 1.0.2 | Initial version of RabbitMQ |
| 1.0.7 | Connect with URI added |
| 1.0.8 | Add Create and Durable options in WriteMessage. Remove declaring queue in ReadMessage operation |
| 1.1.0 | Fix nacking while reading multiple messages before it read same message multiple times, because of immediately nacking |
| 1.2.0 | Write to exchange, but does not implement creating exchange on fly. |
| 1.3.0 | Message persistence is set to true if durable parameter is true. |
