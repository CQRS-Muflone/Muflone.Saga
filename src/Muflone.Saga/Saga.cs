using System;
using System.Collections.Generic;
using Muflone.Saga.Persistence;

namespace Muflone.Saga
{
	public abstract class Saga<TSagaState> : ISaga<TSagaState>, IEquatable<ISaga> where TSagaState : class, new()
	{
		protected readonly ISagaRepository<TSagaState> Repository;
		public ISagaId CorrelationId { get; set; }
		public IDictionary<string, object> Headers { get; set; }
		public TSagaState SagaState { get; set; }

		protected Saga(ISagaRepository<TSagaState> repository)
		{
			Repository = repository;
		}

		public virtual bool Equals(ISaga other) => null != other && GetType() == other.GetType() && other.CorrelationId.GetType() == CorrelationId.GetType() && other.CorrelationId.Value == CorrelationId.Value;
		public override bool Equals(object obj) => Equals(obj as ISaga);
		public override int GetHashCode() => CorrelationId.Value.GetHashCode();

		public static bool operator ==(Saga<TSagaState> saga1, Saga<TSagaState> saga2)
		{
			if ((object)saga1 == null && (object)saga2 == null)
				return true;

			if ((object)saga1 == null || (object)saga2 == null)
				return false;

			return saga1.GetType() == saga2.GetType() && saga1.CorrelationId.GetType() == saga2.CorrelationId.GetType() &&
						 saga1.CorrelationId.Value == saga2.CorrelationId.Value;
		}
		public static bool operator !=(Saga<TSagaState> saga1, Saga<TSagaState> saga2) => !(saga1 == saga2);

	}
}
