# MassTransit + Dapr 🚧🏗👷‍♂️


## Backlog

* testing
* ci/cd
* nuget ? 
* Handle deserialization properly, should not commit before message is at least delivered to consumers
* serializer selection strategy - support multiple formats (plain/text, application/json, application/cloudevents+json)
* Producer integration; event publishing
* implement proper cloudevent context
* bug: state machine publishes multiple times (2)
* integrate with MassTransitHostedService (currently, we start host manually to ensure registration order)
* integrate other building blocks: store
