
public class AutoGenMicroserviceClientSample
{
	/*
	 * INTRODUCTION
	 * ---------------------------------------------------
	 * 
	 * This is a sample script that describes how you can use the auto-generated C#MS clients to have your Unity code, talk to your C#MSs.
	 *
	 * This script is in a common assembly definition, that is auto-referenced. This means that all auto-generated C#MS Clients that we create in this directory are accessible by any and all
	 * of your project by default. If you don't want this, you can disable the auto-reference here and manually add this AsmDef to any other AsmDef containing code that needs to talk to your
	 * C#MSs.
	 *
	 
	 * HOW TO TALK TO MICROSERVICES
	 * ---------------------------------------------------
	 * There are multiple ways you can use the generated code here:
	 * - Via Dependency Injection and BeamContextSystems
	 * - Via the BeamContext.Microservices().GetClient<TClientType>() or BeamContext.Microservices().{ServiceName}() extension methods.
	 * - Via "new ServiceName(beamContext)" passing in the BeamContext representing the authenticated player that wishes to talk to the C#MS.
	 *
	 * All of these ways give you a working C#MS client instance ready to use and talk to your local or remote C#MS. (If there's no locally deployed C#MS, it try to use the remote one instead).  
	 *
	 
	 * DEPENDENCY INJECTION AND BEAM CONTEXT SYSTEMS
	 * ---------------------------------------------------
	 *
	 * Any C# class with the following attributes and static declaration can register itself to be a BeamContext system, in this case a singleton one. 
	 
		[BeamContextSystem]
	    public class SomeGameSystem
	    {
	        /// <summary>
	        /// Add this system as a singleton dependency of every <see cref="BeamContext"/>.
	        /// </summary>
	        [RegisterBeamableDependencies()]
	        public static void RegisterService(IDependencyBuilder builder) => builder.AddScoped<SomeGameSystem>();
	    }
	    
	 * Any BeamContextSystem can declare, via its constructors, other BeamContextSystems it cares about. The Beamable Dependency Injection system will provide an instance of type based on the
	 * registered services.
	 
		public SomeGameSystem(SomeMicroserviceClient someMicroservice)
        {
            _someMicroservice = someMicroservice;            
        }
        
	 * The above constructor would get an instance of the auto-generated SomeMicroserviceClient into the SomeGameSystem class.
	 * This is handled automatically by Beamable's DI-framework (it is heavily inspired by Microsoft's own .NET C# DI framework). 
	 
	 * EXTENSION METHODS & NEW INSTANCES
	 * ---------------------------------------------------
	 * The BeamContext class has an Microservices() method that returns a MicroserviceClients object. This object has extension methods on them that we auto-generate for each C#MS.
	 * These return the existing C#MS client instance in the BeamContext's ServiceProvider that you can use to talk to the C#MS as the authenticated user of that BeamContext.
	 
		BeamContext.Default.Microservices().SomeMicroservice();	
		
	 * Alternatively, you can use the GetClient<TMicroserviceClientType>() to retrieve the same instance.
	 
		BeamContext.Default.Microservices().GetClient<SomeMicroserviceClient>();
	 
	 * Finally, if you wish to, you can simply create a new instance of a C#MS client normally (via "new") and pass in the BeamContext you want it to use. Not passing in a BeamContext
	 * will make the client use the BeamContext.Default property.
	 
		"new SomeMicroserviceClient(BeamContext.Default)" is the same as "new SomeMicroserviceClient()"
     
     * These are all the ways you can start talking to your C#MSs using our auto-generated Clients.
	 * 
	 */
}

