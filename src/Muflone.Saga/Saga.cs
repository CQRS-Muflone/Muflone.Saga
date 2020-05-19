using System;
using Muflone.Saga.Persistence;

namespace Muflone.Saga
{
	public abstract class SagaBase<TSagaData> : ISaga<TSagaData>, IEquatable<ISaga> where TSagaData : class
	{
		protected readonly ISagaRepository Repository;

		protected SagaBase(ISagaRepository repository)
		{
			Repository = repository ?? throw new ArgumentNullException(nameof(repository));
		}

		public ISagaId Id { get; set; }

		public TSagaData SagaData { get; set; }

		public virtual bool Equals(ISaga other) => null != other && GetType() == other.GetType() && other.Id.GetType() == Id.GetType() && other.Id.Value == Id.Value;
		public override bool Equals(object obj) => Equals(obj as ISaga);
		public override int GetHashCode() => Id.Value.GetHashCode();

		public static bool operator ==(SagaBase<TSagaData> saga1, SagaBase<TSagaData> saga2)
		{
			if ((object)saga1 == null && (object)saga2 == null)
				return true;

			if ((object)saga1 == null || (object)saga2 == null)
				return false;

			return saga1.GetType() == saga2.GetType() && saga1.Id.GetType() == saga2.Id.GetType() &&
						 saga1.Id.Value == saga2.Id.Value;
		}
		public static bool operator !=(SagaBase<TSagaData> saga1, SagaBase<TSagaData> saga2) => !(saga1 == saga2);

	}
}
