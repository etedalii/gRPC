# gRPC

# .NET 8

The gRPC service consists of five methods: Create, Read (for individual items), List (for multiple items), Update, and Delete. I utilize JSON transcoding, a novel functionality introduced in .NET 7, enabling our gRPC service to function as a REST-based API. This integration allows web-based endpoints to access our service, while also maintaining compatibility with native gRPC clients.