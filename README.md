m-r
===

While in CQRS commands and queries live in disparate systems, they will be represented as a unified resource at the API Layer.
This prototype exposes Greg Young's m-r sample - which has been the de-facto CRQS+ES (ES = Event Sourcing) sample in the community - through a RESTful interface.

This prototype exemplifies:

Forming the API's Public Domain abstracing Server/BoundedCountext's internal domain. Public domain is composed of DTOs and separate commands.
Service has been exposed as resources. Resource accept GET, POST, DELETE and PUT requests - currently OPTIONS is not implemented.
GET /api/InventoryItem [gets all items]
GET /api/InventoryItem/{id} [gets detail of a single item]
POST /api/InventoryItem [creates an item]
POST /api/InventoryItem/{id}* [checks in stock items to the inventory]
POST /api/InventoryItem/{id}* [removes stock items from the inventory]
PUT /api/InventoryItem/{id} [renames an item]
DELETE /api/InventoryItem/{id} [de-activates an item]
Operations marked with the * above require passing the command type as described by the 5 levels of media type. This helps to avoid RPC-style URLs where a verb is defined on the top of a resource: so instead of /api/InventoryItem/{id}/AddToStock, we send a request with media type application/json;domain-model=CheckInItemsToInventoryCommand. This also moves away from the common misconception that HTTP Verbs must be mapped to CRUD.
Exposing ES concurrency through HTTP's ETag and If-Match and If-None-Match conditional PUT and GET requests.
Enabling caching on the top of single resources and returning 304 for resources that are not modified. Also returning 412 (PreconditionFailed) on unsuccessful If-Match conditional PUT calls.
As per RFC 2616, ETag has been opacified so that the client cannot guess the values - this would have been possible if we had exposed version numbers directly.
Other HTTP-level semantics such as status codes, populating Location header after POST, etc.
