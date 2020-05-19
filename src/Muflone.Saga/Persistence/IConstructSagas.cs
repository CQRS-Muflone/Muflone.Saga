using System;

namespace Muflone.Saga.Persistence
{
  public interface IConstructSagas
  {
    ISaga Build(Type type, string id);
  }
}