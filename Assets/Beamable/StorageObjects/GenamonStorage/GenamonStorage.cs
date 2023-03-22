using Beamable.Common;
using MongoDB.Driver;

namespace Beamable.Server
{
	[StorageObject("GenamonStorage")]
	public class GenamonStorage : MongoStorageObject
	{
	}

	public static class GenamonStorageExtension
	{
		/// <summary>
		/// Get an authenticated MongoDB instance for GenamonStorage
		/// </summary>
		/// <returns></returns>
		public static Promise<IMongoDatabase> GenamonStorageDatabase(this IStorageObjectConnectionProvider provider)
			=> provider.GetDatabase<GenamonStorage>();

		/// <summary>
		/// Gets a MongoDB collection from GenamonStorage by the requested name, and uses the given mapping class.
		/// If you don't want to pass in a name, consider using <see cref="GenamonStorageCollection{TCollection}()"/>
		/// </summary>
		/// <param name="name">The name of the collection</param>
		/// <typeparam name="TCollection">The type of the mapping class</typeparam>
		/// <returns>When the promise completes, you'll have an authorized collection</returns>
		public static Promise<IMongoCollection<TCollection>> GenamonStorageCollection<TCollection>(
			this IStorageObjectConnectionProvider provider, string name)
			where TCollection : StorageDocument
			=> provider.GetCollection<GenamonStorage, TCollection>(name);

		/// <summary>
		/// Gets a MongoDB collection from GenamonStorage by the requested name, and uses the given mapping class.
		/// If you want to control the collection name separate from the class name, consider using <see cref="GenamonStorageCollection{TCollection}(string)"/>
		/// </summary>
		/// <param name="name">The name of the collection</param>
		/// <typeparam name="TCollection">The type of the mapping class</typeparam>
		/// <returns>When the promise completes, you'll have an authorized collection</returns>
		public static Promise<IMongoCollection<TCollection>> GenamonStorageCollection<TCollection>(
			this IStorageObjectConnectionProvider provider)
			where TCollection : StorageDocument
			=> provider.GetCollection<GenamonStorage, TCollection>();
	}
}
