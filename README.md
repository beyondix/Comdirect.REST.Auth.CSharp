# Comdirect.REST.Auth.CSharp

This package is an abstraction of the authentication flow which is necessary to make calls to the comdirect REST API. 
NuGet Package is provided soon. An abstraction of the data endpoints will be provided soon in an own repository.

## Attention 

If you just ask five times for a TAN challenge without activating that TAN your account is blocked. So please make sure you activate all your TAN's!

You need to activate the TAN in your comdirect PhotoTAN app. If you provide the wrong TAN three times in a row your account is blocked. 
After two not successfull attempts you can reset the error counter at the comdirect website.

## Usage

See sample: `Comdirect.Auth.CSharp.Sample`. This is a console app where you can test the code.
Just add your credentials in the sample and try it out :-)
