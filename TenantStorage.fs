module TenantStorage

open Pulumi
open Pulumi.Aws.S3
open Pulumi.Aws.S3.Inputs
open Feliz.ViewEngine

let create (tenant: string, license: string) = 
    let siteBucket = Bucket($"storage-{tenant}", BucketArgs(Website=BucketWebsiteArgs(IndexDocument="index.html")))

    let indexContent = Html.html [
        Html.head [
            Html.title [ Html.text "Hello S3!" ]
            Html.meta [ prop.charset "utf-8" ]
        ]

        Html.body [
            Html.p $"This is a static website created for {tenant}!"
            Html.p $"License: {license}"
            Html.p [
                Html.text "Made with ❤️ with "
                Html.a [ 
                    prop.text "Pulumi"
                    prop.href "https://pulumi.com" 
                ]
            ]
        ]
    ]

    let indexContentSource = StringAsset(Render.htmlView indexContent) :> AssetOrArchive

    let indexHtml = BucketObject("index", BucketObjectArgs(
        Bucket=siteBucket.Id,
        Source=indexContentSource,
        Key="index.html",
        ContentType="text/html; charset=utf-8",
        Acl="public-read"
    ))

    outputs [ 
        "websiteUrl" ==> siteBucket.WebsiteEndpoint
        "readme" ==> String.concat "\n" [
            $"## Static website for `{tenant}`"
            "See it live [from here](http://${outputs.websiteUrl})"
        ]
    ]