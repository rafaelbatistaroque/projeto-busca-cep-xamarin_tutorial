using curso_xamarin.domains;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace curso_xamarin.ViewModels
{
    class BuscaCepViewModel : ViewModelBase
    {
        private Command _buscarEnderecoComando;
        private Endereco _enderecoDto = null;
        private string _cep;

        public string Cep
        {
            get => _cep;
            set
            {
                _cep = value;
                OnPropertyChanged();
                BuscarEnderecoComando.ChangeCanExecute();
            }
        }
        public string Logradouro { get => _enderecoDto?.Logradouro; }
        public string Complemento { get => _enderecoDto?.Complemento; }
        public string Bairro { get => _enderecoDto?.Bairro; }
        public string Localidade {get => _enderecoDto?.Localidade; }
        public string Uf{ get => _enderecoDto?.Uf; }
        public bool TemCep { get => !(_enderecoDto is null); }
        public Command BuscarEnderecoComando => _buscarEnderecoComando
            ?? (_buscarEnderecoComando = new Command(async
                () => await BuscarEndereco(),
                () => ValidarBuscarEndereco()
            ));
        
        public BuscaCepViewModel() { }

        private async Task BuscarEndereco()
        {
            try
            {
                if (Ocupado) return;

                Ocupado = true;
                BuscarEnderecoComando.ChangeCanExecute();

                using (var client = new HttpClient())
                {
                    var resposta = await client.GetAsync($"https://viacep.com.br/ws/{Cep}/json/");

                    resposta.EnsureSuccessStatusCode();

                    var conteudoResposta = await resposta.Content.ReadAsStringAsync();

                    _enderecoDto = JsonConvert.DeserializeObject<Endereco>(conteudoResposta);

                    if (_enderecoDto.Erro) throw new Exception("Endereço não encontrado.");
                }
            }
            catch (Exception erro)
            {
                _enderecoDto = null;
                await App.Current.MainPage.DisplayAlert("Aviso:", $"{erro.Message}", "Beleza, então");
            }
            finally
            {
                OnPropertyChanged(nameof(TemCep));
                OnPropertyChanged(nameof(Logradouro));
                OnPropertyChanged(nameof(Complemento));
                OnPropertyChanged(nameof(Bairro));
                OnPropertyChanged(nameof(Localidade));
                OnPropertyChanged(nameof(Uf));
                Ocupado = false;
                BuscarEnderecoComando.ChangeCanExecute();
            }
        }
        private bool ValidarBuscarEndereco() => !string.IsNullOrWhiteSpace(Cep) && Cep.Length == 8 && Desocupado;
    }
}
