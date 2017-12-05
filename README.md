# FaultlessExecution
Library providing short-hand and extensibility for trapping exceptions

## Purpose
To provide a mechanism declutter code that might require lots of try..catch as well as provide a way to more easily process exceptions on a line by line basis rather than relying on global error handlers. 

Hopefully this library encourages you, as it has me, to solve less than desirable try/catch solutions that I generally find myself facing
* Wrapping large chunks of code in a single try/catch, making it difficult to recover from an exception because too much code has been encompassed 
* Skipping try/catches altogether because it just seems tedious and pasting the same exact code in every "catch" to handle the exception seems redundant and annoying
* Rely on a single global error handler for your entire application, and just accept that if an error occurs, the app (or web request) is pretty much a lost cause until restarted


## Usage
new up or inject an instance of the FaultlessExecutionService
```c#
private FaultlessExecution.Abstractions.IFaultlessExecutionService _faultlessExecutionService
    = new FaultlessExecution.FaultlessExecutionService();
```        

Wrap your call to an external database/repo/api/service in a TryExecute so that no execptions are thrown
```c#
var peopleResult = _faultlessExecutionService
    .TryExecute(() => _database.GetAllThePeopleInTheWorld());
```

Choose what to do if the code ran without error and what to do if the code did throw an error
```c#
 if (peopleResult.WasSuccessful)
      LoadPersonList(peopleResult.ReturnValue);
 else
      HandleException(peopleResult.Exception);
```
### Some Helpful Extensions
While the first example helps a little, we can use extensions to simplify the code even more
```c#
_faultlessExecutionService
    .TryExecute(() => _database.GetAllThePeopleInTheWorld())
    .OnException((result) => this.HandleException(result.Exception))
    .OnSuccess((result) => this.LoadPersonList(result.ReturnValue));
```

### Retry logic
Since the code we are passing to the service is Func<T> or Action, we can make use of extension methods for Retry, which can simply re-invoke the code if necessary.
```c#
 _faultlessExecutionService
    .TryExecute(() => _database.GetAllThePeopleInTheWorld())
    .RetryOnceIf((result) => result.Exception is TimeoutException)
    .OnException((result) => this.HandleException(result.Exception))
    .OnSuccess((result) => this.LoadPersonList(result.ReturnValue));
```

### Async 
```c#
 var peopleResult = await _faultlessExecutionService
     .TryExecute(() => _database.GetAllThePeopleInTheWorld());
 if(peopleResult.WasSuccessful)
    this.LoadPersonList(result.ReturnValue);
 else //Bail because we cannot finish loading    
    return;
```
## Don't copy and paste the OnException code
One of the most unrealized uses of this service, is the ability to write your own super-simple handler for all exceptions; this prevents copying and pasting the OnException block everytime you use the service.
Derrive from the FaultlessExecution.FaultlessExecutionService class and you can process any exception however you'd like
```c#
//Please note, i absolutely do not condone this messagebox implementation, 
//  but it's simple and illustrates the point
public class ShowMessageBoxFaultlesExecutionService : FaultlessExecution.FaultlessExecutionService
{
    protected override void OnException(Exception ex)
    {
        //Log the error somewhere, maybe NLog or Log4Net
        _logger.Log(ex);
        
        //show the user the error because otherwise they will just wonder why the view did not load
        System.Windows.MessageBox.Show(ex.Message);
    }
}
```

With our new MessageBox error handler, we change our IOC Container to inject ShowMessageBoxFaultlesExecutionService in for our IFaultlessExecutionService (or new it up if DI is not your cup of tea)
```c#
//no need for OnException, because our custom implementation already handles the errors
 _faultlessExecutionService
    .TryExecute(() => _database.GetAllThePeopleInTheWorld())
    .OnSuccess((result) => this.LoadPersonList(result.ReturnValue));
```

### Sample Exception Handler Implementation for Wpf/Xamarin Forms
In Wpf and Xamarin Forms I do show the error to the user, with an implementation like this because it allows me to completely separate any UI invocation, making it super testable (unlike the Messagebox implementation above)
```c#
//This interface allows me to inject either the base level IFaultlessExecutionService into my class 
//  or if it's a UI component this IViewModelFaultHandler which will show something to the user
public interface IViewModelFaultHandler : IFaultlessExecutionService
{
}

 public class ViewModelFaultHandler : FaultlessExecutionService, IViewModelFaultHandler
    {
        private Contracts.INavigator _navigator;
        public ViewModelFaultHandler(Contracts.INavigator navigator)
        {
            _navigator = navigator;
        }

        protected override void OnException(Exception ex)
        {
            var connectionException = ex as Api.Contracts.NoConnectionException;
            if (connectionException != null)
            {
                _navigator.PushModalAsync<ViewModels.NoConnectionViewModel>();
                return;
            }

            var httpException = ex as Api.Contracts.HttpResponseException;
            if (httpException != null)
            {
                _navigator.PushModalAsync<ViewModels.ErrorViewModel>(initAction: (vm) =>
                {
                    vm.Message = $"Http error code {httpException.StatusCode}";
                    vm.ErrorMessageStack = httpException.Message;
                    vm.ShowingDetails = true;
                });
                return;

            }

            var timeoutException = ex as Api.Contracts.ServiceCallTimeoutException;
            if (timeoutException != null)
            {

                _navigator.PushModalAsync<ViewModels.ErrorViewModel>(initAction: (vm) =>
                {
                    vm.Message = "Connection Timed Out";
                    vm.ErrorMessageStack = "Looks like the server is taking too long to respond, this can be caused by poor connectivity or an error with our servers. Please try again in a while";
                    vm.ShowingDetails = true;
                });
                return;
            }

            _navigator.PushModalAsync<ViewModels.ErrorViewModel>(initAction: (vm) => vm.LoadFromException(ex));
        }
  }
```

## I have a global error handler. I don't need this.
Fantastic! Your global error handler will catch all the errors, allow you to log and show them, but that still does not allow you to _elegantly_ recover from calls that you know are potentially risky.

When you make a call to get data from the database, or from a Rest Api, or anywhere that there might be connection issues or timeouts, are you comfortable with the global error handler?  Once you hit your global handler, your application is in a pretty unknown state because you bailed from all code without a chance to leave the user in a good place.

A good use of this library is to handle all errors, and do a bit of cleanup before bailing. Maybe there are some pieces of data that your view can live without, while others warrent you telling the user what happened and closing out the window/page.

## Why not just use try/catch around every line that might error
If you are disciplined enough to put try catch around every external resource call, and copy/paste the same "catch" block in every place, then you are far more disciplined than I.

## Installing
TODO- provide nuget package info
