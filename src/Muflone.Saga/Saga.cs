using System;
using System.Collections.Generic;
using Muflone.Saga.Persistence;

namespace Muflone.Saga
{
	public abstract class Saga<TSagaData> : ISaga<TSagaData>, IEquatable<ISaga> where TSagaData : class
	{
		protected readonly ISagaRepository Repository;
		public ISagaId Id { get; protected set; }
		public IDictionary<string, object> Headers { get; set; }
		public TSagaData SagaData { get; set; }

		protected Saga(ISagaRepository repository)
		{
			Repository = repository;
		}









		public virtual bool Equals(ISaga other) => null != other && GetType() == other.GetType() && other.Id.GetType() == Id.GetType() && other.Id.Value == Id.Value;
		public override bool Equals(object obj) => Equals(obj as ISaga);
		public override int GetHashCode() => Id.Value.GetHashCode();

		public static bool operator ==(Saga<TSagaData> saga1, Saga<TSagaData> saga2)
		{
			if ((object)saga1 == null && (object)saga2 == null)
				return true;

			if ((object)saga1 == null || (object)saga2 == null)
				return false;

			return saga1.GetType() == saga2.GetType() && saga1.Id.GetType() == saga2.Id.GetType() &&
						 saga1.Id.Value == saga2.Id.Value;
		}
		public static bool operator !=(Saga<TSagaData> saga1, Saga<TSagaData> saga2) => !(saga1 == saga2);

	}
}
