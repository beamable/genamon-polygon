//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Beamable.Server.Clients
{
    using System;
    using Beamable.Platform.SDK;
    using Beamable.Server;
    
    
    /// <summary> A generated client for <see cref="Beamable.Microservices.GenamonService"/> </summary
    public sealed class GenamonServiceClient : MicroserviceClient, Beamable.Common.IHaveServiceName
    {
        
        public GenamonServiceClient(BeamContext context = null) : 
                base(context)
        {
        }
        
        public string ServiceName
        {
            get
            {
                return "GenamonService";
            }
        }
        
        /// <summary>
        /// Call the GetStatus method on the GenamonService microservice
        /// <see cref="Beamable.Microservices.GenamonService.GetStatus"/>
        /// </summary>
        public Beamable.Common.Promise<GetStatusResponse> GetStatus()
        {
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            return this.Request<GetStatusResponse>("GenamonService", "status", serializedFields);
        }
        
        /// <summary>
        /// Call the Generate method on the GenamonService microservice
        /// <see cref="Beamable.Microservices.GenamonService.Generate"/>
        /// </summary>
        public Beamable.Common.Promise<System.Threading.Tasks.Task> Generate()
        {
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            return this.Request<System.Threading.Tasks.Task>("GenamonService", "Generate", serializedFields);
        }
        
        /// <summary>
        /// Call the Collect method on the GenamonService microservice
        /// <see cref="Beamable.Microservices.GenamonService.Collect"/>
        /// </summary>
        public Beamable.Common.Promise<Beamable.Common.Unit> Collect(string genamonId, Beamable.Common.Inventory.ItemRef itemRef)
        {
            object raw_genamonId = genamonId;
            object raw_itemRef = itemRef;
            System.Collections.Generic.Dictionary<string, object> serializedFields = new System.Collections.Generic.Dictionary<string, object>();
            serializedFields.Add("genamonId", raw_genamonId);
            serializedFields.Add("itemRef", raw_itemRef);
            return this.Request<Beamable.Common.Unit>("GenamonService", "collect", serializedFields);
        }
    }
    
    internal sealed class MicroserviceParametersGenamonServiceClient
    {
        
        [System.SerializableAttribute()]
        internal sealed class ParameterSystem_String : MicroserviceClientDataWrapper<string>
        {
        }
        
        [System.SerializableAttribute()]
        internal sealed class ParameterBeamable_Common_Inventory_ItemRef : MicroserviceClientDataWrapper<Beamable.Common.Inventory.ItemRef>
        {
        }
    }
    
    [BeamContextSystemAttribute()]
    public static class ExtensionsForGenamonServiceClient
    {
        
        [Beamable.Common.Dependencies.RegisterBeamableDependenciesAttribute()]
        public static void RegisterService(Beamable.Common.Dependencies.IDependencyBuilder builder)
        {
            builder.AddScoped<GenamonServiceClient>();
        }
        
        public static GenamonServiceClient GenamonService(this Beamable.Server.MicroserviceClients clients)
        {
            return clients.GetClient<GenamonServiceClient>();
        }
    }
}
