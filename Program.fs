open Falco
open Falco.Routing
open Falco.HostBuilder
open Microsoft.AspNetCore.Http
open System.Threading.Tasks
open Pulumi.Automation
open System

type CustomerInfo = { tenant: string; license: string }

let stdout (text: string) = Console.WriteLine(text)

let configureWorkspace(request: CustomerInfo) = task {
    let program = PulumiFn.Create(fun _ -> TenantStorage.create(request.tenant, request.license))
    let stackArgs = InlineProgramArgs("tenant-infra", request.tenant, program)
    let! workspace = LocalWorkspace.CreateOrSelectStackAsync(stackArgs)
    do! workspace.SetConfigAsync("aws:region", ConfigValue("eu-west-1"))
    return workspace
}

let createTenantInfra (request: CustomerInfo) = task {
    let! workspace = configureWorkspace request
    let! results = workspace.UpAsync(UpOptions(OnStandardOutput=stdout))
    return dict [ "message" ==> $"Stack created for {request.tenant}" ]
}

let removeTenantInfra (request: CustomerInfo) = task {
    let! workspace = configureWorkspace request
    let! results = workspace.DestroyAsync(DestroyOptions(OnStandardOutput=stdout))
    return dict [ "message" ==> $"Stack removed for {request.tenant}" ]
} 

let onboard : HttpHandler = 
    let handler (request: CustomerInfo) (ctx: HttpContext) : Task =
        task {
            let! result = createTenantInfra request
            do! Response.ofJson result ctx
        }
    
    Request.mapJson handler

let offboard : HttpHandler = 
    let handler (request: CustomerInfo) (ctx: HttpContext) : Task = 
        task {
            let! result = removeTenantInfra request
            do! Response.ofJson result ctx
        }

    Request.mapJson handler

webHost [||] {
    endpoints [
        get "/" (Response.ofPlainText "Hello, Pulumi Automation")
        post "/onboard" onboard
        post "/offboard" offboard
    ]
}