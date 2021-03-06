using System.Collections.Generic;

namespace Elders.Cronus.EventStore
{
    public interface IEventStorePlayer
    {
        /// <summary>
        /// Loads all aggregate commits. The commits are unordered.
        /// </summary>
        /// <param name="batchSize">Size of the batch.</param>
        /// <returns></returns>
        IEnumerable<AggregateCommit> LoadAggregateCommits(int batchSize = 5000);

        /// <summary>
        /// Loads all aggregate commits. The commits are unordered.
        /// </summary>
        /// <param name="batchSize">Size of the batch.</param>
        /// <returns></returns>
        IEnumerable<AggregateCommitRaw> LoadAggregateCommitsRaw(int batchSize = 5000);

        /// <summary>
        /// Loads all aggregate commits. The commits are unordered.
        /// </summary>
        /// <param name="batchSize">Size of the batch.</param>
        /// <returns></returns>
        LoadAggregateCommitsResult LoadAggregateCommits(string paginationToken, int batchSize = 5000);
    }

    public interface IEventStorePlayer<TSettings> : IEventStorePlayer
        where TSettings : class
    { }

    public class LoadAggregateCommitsResult
    {
        public LoadAggregateCommitsResult()
        {
            Commits = new List<AggregateCommit>();
        }

        public string PaginationToken { get; set; }

        public List<AggregateCommit> Commits { get; set; }
    }
}
