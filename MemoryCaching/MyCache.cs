/*
 *  FILENAME        : MyCache.cs
 *  PROJECT         : MemoryCaching
 *  PROGRAMMER      : Jody Markic
 *  FIRST VERSION   : 2/21/2018
 *  DESCRIPTION     : This a wrapper class for MemoryCache class and related objects in .NET 4.0 application,
 *                    This code back is heavily influenced from the code written by Dinesh K Mandal found at
 *                    https://www.codeproject.com/articles/290935/using-memorycache-in
 *                    This class supports methods to add, retrieve, remove, and log removal of Objects to a from a MemoryCache.
 */
 
//included namespaces.
using System;
using System.Collections.Generic;
using System.Runtime.Caching;

//project namespace.
namespace MemoryCaching
{
    //enum to help evaluate the expirary set to a CacheItem.
    public enum MyCachePriority
    {
        Default,
        NonRemovable
    }

    //
    //  CLASS       : MyCache
    //  DESCRIPTION : This a wrapper class for MemoryCache class and related objects in .NET 4.0 application,
    //                This code back is heavily influenced from the code written by Dinesh K Mandal found at
    //                https://www.codeproject.com/articles/290935/using-memorycache-in
    //                This class supports methods to add, retrieve, remove, and log removal of Objects to a from a MemoryCache.
    //
    public class MyCache
    {
        //private variables
        private static ObjectCache myCache = MemoryCache.Default;
        private CacheItemPolicy myPolicy = null;
        private CacheEntryRemovedCallback callback = null;

        //
        //  METHOD: CheckCache
        //  DESCRIPTION: This method checks if the cache hold data with a specific keyword
        //  PARAMETERS: String CacheItemIdentifier
        //  RETURNS: bool result
        //
        public bool CheckCache(String CacheItemIdentifier)
        {
            bool result = false;

            if (myCache.Contains(CacheItemIdentifier))
            {
                result = true;
            }

            return result;
        }

        //
        //  METHOD: AddToCache
        //  DESCRIPTION: This method adds an item to the cache setting a removal callback, expiration, priority, and filepath
        //  PARAMETERS: String CacheItemIdentifier, Object CacheItem, MyCachePriority CacheItemPriority, List<String> FilePath
        //  RETURNS: void
        //
        public void AddToCache(String CacheItemIdentifier, Object CacheItem, MyCachePriority CacheItemPriority, List<String> FilePath)
        {
            //instantiate a callback and Policy object.
            callback = new CacheEntryRemovedCallback(this.MyCachedItemRemovedCallback);
            myPolicy = new CacheItemPolicy();

           //ternary operator. 
           //evaluates if our enum values is Default if so set Policy priority to default othewise make it notremovable
            myPolicy.Priority = (CacheItemPriority == MyCachePriority.Default)
                     ? System.Runtime.Caching.CacheItemPriority.Default 
                     : System.Runtime.Caching.CacheItemPriority.NotRemovable;
            //set expiration for cached item
            myPolicy.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(120.00);
            //assign callback 
            myPolicy.RemovedCallback = callback;
            //assign file paths
            myPolicy.ChangeMonitors.Add(new HostFileChangeMonitor(FilePath));

            //store in Cache
            myCache.Set(CacheItemIdentifier, CacheItem, myPolicy);
        }

        //
        //  METHOD: GetCachedItem
        //  DESCRIPTION: This method retrieves a cached item with a specific identifier
        //  PARAMETERS: String CacheItemIdentifier
        //  RETURNS: Object GetCachedItem
        //
        public Object GetCachedItem(String CacheItemIdentifier)
        {
            //return CacheItem
            return myCache[CacheItemIdentifier] as Object;
        }

        //
        //  METHOD: RemoveCachedItem
        //  DESCRIPTION: This method removes a cached item with a specific identifier
        //  PARAMETERS: String CacheItemIdentifier
        //  RETURNS: void
        //
        public void RemoveCachedItem(String CacheItemIdentifier)
        {
            //check if cache item exists
            if (myCache.Contains(CacheItemIdentifier))
            {
                //remove cache item
                myCache.Remove(CacheItemIdentifier);
            }
        }


        //
        //  METHOD: MyCachedItemRemovedCallback
        //  DESCRIPTION: callback on removal of a item currently stored in cache, logs event to a txt file.
        //  PARAMETERS: CacheEntryRemovedArguments arguements
        //  RETURNS: void
        //
        private void MyCachedItemRemovedCallback(CacheEntryRemovedArguments arguements)
        {
            //format string to log to a text file.
            String log = String.Concat("Reason: ", arguements.RemovedReason.ToString(),
                                       "| Key-name: ", arguements.CacheItem.Key,
                                       " | Value-Object: ", arguements.CacheItem.Value.ToString());

            //could write log out to a text file to keep logs of removal.
        }
    }
}
