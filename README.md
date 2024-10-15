# Muflone.Saga
Basic implementation of a saga (not event sourced at the moment)
 
### Install ###
`Install-Package Muflone.Saga`

### Breaking change in version 8.1.2 ###
Inside the Saga implementation we integrated IStartedBy to avoid error by forgotting to implement it

So, for example, the code changes from

```C#
public class MySaga : Saga<MySaga.MyData>, IStartedBy<FakeStartingCommand>, ISagaEventHandlerAsync<FakeResponse>, ISagaEventHandlerAsync<FakeResponseError>`
```

to

```C#
public class MySaga : Saga<FakeStartingCommand, MySaga.MyData>,     ISagaEventHandlerAsync<FakeResponse>, ISagaEventHandlerAsync<FakeResponseError>
```

and the implementation of the interface changes feom

```C#
public async Task StartedByAsync(FakeStartingCommand command)
{
}
```

to

```C#
public override async Task StartedByAsync(FakeStartingCommand command)
{
}
```
