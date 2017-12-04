using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FaultlessExecution.Extensions;//gives you access to OnSuccess, OnException, Retry, RetryIf, etc
using System.Collections.ObjectModel;

namespace FaultlessExecutionSample
{
    public class MainViewModel : NotificationObjectBase
    {
        private DemoServices.AwesomeSqlDatabase _database = new DemoServices.AwesomeSqlDatabase();
        private DemoServices.AmazingWebApi _webApi = new DemoServices.AmazingWebApi();
        private DemoServices.TheMysteriousCloud _cloud = new DemoServices.TheMysteriousCloud();
        private DemoServices.YourFileSystem _fileSystem = new DemoServices.YourFileSystem();

        private FaultlessExecution.Abstractions.IFaultlessExecutionService _faultlessExecutionService;

        public void Load()
        {
            this.PeopleList = new ObservableCollection<DemoServices.Person>();
            this.CityList = new ObservableCollection<DemoServices.City>();
            this.AnswerList = new ObservableCollection<DemoServices.Answer>();
            this.ErrorList = new ObservableCollection<string>();

            if (UseGenericExceptionHandler)
                _faultlessExecutionService = new ShowMessageBoxFaultlesExecutionService(); //this demonstrates how you might write a generic handler so that you don't need to worry about writing .onexeption handlers for all your calls
            else
                _faultlessExecutionService = new FaultlessExecution.FaultlessExecutionService();

            //simple example of testing that something did not error
            var peopleResult = _faultlessExecutionService.TryExecute(() => _database.GetAllThePeopleInTheWorld());
            if (peopleResult.WasSuccessful)
                LoadPersonList(peopleResult.ReturnValue);
            else
                HandleException(peopleResult.Exception);

            //example of using OnException and/or OnSuccess to perform actions based on whether the call was successful or not
            _faultlessExecutionService.TryExecute(() => _webApi.GetSomeCities())
                .OnException((r) => this.HandleException(r.Exception))
                .OnSuccess((r) => this.LoadCityList(r.ReturnValue));

            //example of using the retry logic to rerun the action if it failed
            var cloudResult = _faultlessExecutionService.TryExecute(() => _cloud.GetAllTheAnswers())
                .Retry(numberOfRetries: 3)
                .OnException((r) => this.HandleException(r.Exception));

            if (cloudResult.WasSuccessful)
                this.LoadAnswerList(cloudResult.ReturnValue);

            //example of using the retryIf logic to rerun the action if some condition is met
            _faultlessExecutionService.TryExecute(() => _fileSystem.CheckAllOfYourFiles())
                .RetryOnceIf((result) => result.Exception is TimeoutException)
                .OnException((r) => this.HandleException(r.Exception));
        }

        public async void LoadAsync()
        {
            var peopleResult = await _faultlessExecutionService.TryExecuteAsync(() => _database.GetAllThePeopleInTheWorldAsync());
            if (!peopleResult.WasSuccessful)
                return;

            this.LoadPersonList(peopleResult.ReturnValue);

            var cityResult = await _faultlessExecutionService.TryExecuteAsync(() => _webApi.GetSomeCitiesAsync());
            if (!cityResult.WasSuccessful)
                return;

            this.LoadCityList(cityResult.ReturnValue);

            var answerResult = await _faultlessExecutionService.TryExecuteAsync(() => _cloud.GetAllTheAnswersAsync());
            if (!answerResult.WasSuccessful)
                return;

            this.LoadAnswerList(answerResult.ReturnValue);
        }

        public void LoadPersonList(IEnumerable<DemoServices.Person> people) { this.PeopleList = new ObservableCollection<DemoServices.Person>(people); }

        public void LoadCityList(IEnumerable<DemoServices.City> cities) { this.CityList = new ObservableCollection<DemoServices.City>(cities); }
        public void LoadAnswerList(IEnumerable<DemoServices.Answer> answers) { this.AnswerList = new ObservableCollection<DemoServices.Answer>(answers); }

        private void HandleException(Exception exception)
        {
            this.ErrorList.Add(exception.Message);
        }


        #region Binding Properties

        private ObservableCollection<string> _errorList;
        public ObservableCollection<string> ErrorList
        {
            get { return _errorList; }
            set
            {
                if (Object.Equals(_errorList, value) == false)
                {
                    _errorList = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<DemoServices.Person> _peopleList;
        public ObservableCollection<DemoServices.Person> PeopleList
        {
            get { return _peopleList; }
            set
            {
                if (Object.Equals(_peopleList, value) == false)
                {
                    _peopleList = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<DemoServices.City> _cityList;
        public ObservableCollection<DemoServices.City> CityList
        {
            get { return _cityList; }
            set
            {
                if (Object.Equals(_cityList, value) == false)
                {
                    _cityList = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<DemoServices.Answer> _answerList;
        public ObservableCollection<DemoServices.Answer> AnswerList
        {
            get { return _answerList; }
            set
            {
                if (Object.Equals(_answerList, value) == false)
                {
                    _answerList = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool _peopleShouldError;
        public bool PeopleShouldError
        {
            get { return _peopleShouldError; }
            set
            {
                if (Object.Equals(_peopleShouldError, value) == false)
                {
                    _peopleShouldError = value;
                    this.OnPropertyChanged();

                    _database.ThrowError = value;
                }
            }
        }

        private bool _cityShouldError;
        public bool CityShouldError
        {
            get { return _cityShouldError; }
            set
            {
                if (Object.Equals(_cityShouldError, value) == false)
                {
                    _cityShouldError = value;
                    this.OnPropertyChanged();

                    _webApi.ThrowError = value;
                }
            }
        }

        private bool _answerShouldError;
        public bool AnswerShouldError
        {
            get { return _answerShouldError; }
            set
            {
                if (Object.Equals(_answerShouldError, value) == false)
                {
                    _answerShouldError = value;
                    this.OnPropertyChanged();

                    _cloud.ThrowError = value;
                }
            }
        }

        private bool _checkFilesShouldError;
        public bool CheckFilesShouldError
        {
            get { return _checkFilesShouldError; }
            set
            {
                if (Object.Equals(_checkFilesShouldError, value) == false)
                {
                    _checkFilesShouldError = value;
                    this.OnPropertyChanged();

                    _fileSystem.ThrowError = value;
                }
            }
        }

        private bool _useGenericExceptionHandler;
        public bool UseGenericExceptionHandler
        {
            get { return _useGenericExceptionHandler; }
            set
            {
                if (Object.Equals(_useGenericExceptionHandler, value) == false)
                {
                    _useGenericExceptionHandler = value;
                    this.OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Reload Command

        private RelayCommand _reloadCommand;
        public RelayCommand ReloadCommand
        {
            get
            {
                if (_reloadCommand == null)
                    _reloadCommand = new RelayCommand((param) => this.Reload(), (param) => this.CanReload());
                return _reloadCommand;
            }
        }

        public bool CanReload()
        {
            return true;
        }

        /// <summary>
        /// Executes the Reload command 
        /// </summary>
        public void Reload()
        {
            this.Load();
        }

        #endregion

    }
}
