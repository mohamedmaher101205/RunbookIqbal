namespace Runbook.Models
{
    /// <summary>
    /// This class is to manipulate book environment and data mapping
    /// </summary>
    public class BookEnvironment
    {
        /// <summary>
        /// Gets or sets the book environment id  of the book environment
        /// </summary>
        /// <value>BookEnvId</value>
        public int BookEnvId { get; set; }
        /// <summary>
        /// Gets or sets the book id of the book environment
        /// </summary>
        /// <value>BookId</value>
        public string BookId { get; set; }
        /// <summary>
        /// Gets or sets the environment id  of the book environment
        /// </summary>
        /// <value>EnvId</value>
        public string EnvId { get; set; }
    }
}