# Frends.Community.RabbitMQ
Frends task for operating on RabbitMQ queues. Supports reading and writing from queue. 

- [Installing](#installing)
- [Tasks](#tasks)
  - [Write Message](#writemessage)
  - [Read Message](#readmessage)
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
| Queue name | string | Name of the queue | sampleQueue |
| Routing key | string | Routing key (as in RabbitMQ specification) | sampleQueue |
| Subject | string | Subject of the message | Hello Jane |
| Host | string | Address of the server hosting RabbitMQ | localhost |

## ReadMessage

### Task Parameters

| Property             | Type                 | Description                          | Example |
| ---------------------| ---------------------| ------------------------------------ | ----- |
| Queue name | string | Name of the queue | sampleQueue |
| Routing key | string | Routing key (as in RabbitMQ specification) | sampleQueue |
| Read message count | int | Maximum number of messages to be read from queue. It can exceed number of available messages. | 1 |

### Read message sample JSON

{
	"messages": [
		{
			"deliveryTag": 1,
			"body": "AAAA ",
			"messageCount": 2,
			"routingKey": "sampleQueue"
		},
		{
			"deliveryTag": 2,
			"body": "AAEB ",
			"messageCount": 1,
			"routingKey": "sampleQueue"
		},
		{
			"deliveryTag": 3,
			"body": "AAQC ",
			"messageCount": 0,
			"routingKey": "sampleQueue"
		}
	]
}

# License

This project is licensed under the MIT License - see the LICENSE file for details

# Building

Clone a copy of the repo

`git clone https://github.com/CommunityHiQ/Frends.Community.RabbitMQ.git`

Restore dependencies

`nuget restore frends.community.email`

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