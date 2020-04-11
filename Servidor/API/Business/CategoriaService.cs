using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;

namespace API.Business
{
    public interface ICategoriaService
    {
        ServiceResult AddCategoria(CategoriaDTO model);
        Task<ServiceResult> UploadImagem(ImagemDTO model);
        ServiceResult EditarCategoria(CategoriaDTO model);
        ServiceResult<IList<CategoriaDTO>> GetTodasCategorias();
        ServiceResult<CategoriaDTO> GetCategoriaNome(string nome);
        Task<ServiceResult> RemoverCategoria(string nome);
    }

    public class CategoriaService : ICategoriaService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly ICategoriaDAO _categoriaDAO;

        public CategoriaService(IWebHostEnvironment webHostEnviroment, IMapper mapper, ICategoriaDAO categoriaDAO)
        {
            _webHostEnvironment = webHostEnviroment;
            _mapper = mapper;
            _categoriaDAO = categoriaDAO;
        }


        public ServiceResult AddCategoria(CategoriaDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Parametro não pode ser nulo");
            }

            IList<int> erros = new List<int>();

            if (_categoriaDAO.ExisteNomeCategoria(model.Nome))
            {
                erros.Add((int)ErrosEnumeration.NomeCategoriaJaExiste);
            }
            else
            {
                Categoria categoria = _mapper.Map<Categoria>(model);
                _categoriaDAO.AddCategoria(categoria);
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }


        public async Task<ServiceResult> UploadImagem(ImagemDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Parametro não pode ser nulo");
            }

            IList<int> erros = new List<int>();
            Categoria categoria = _categoriaDAO.GetCategoriaNome(model.Nome);

            if (categoria == null)
            {
                erros.Add((int)ErrosEnumeration.CategoriaNaoExiste);
            }
            else
            {
                string fileExtension = Path.GetExtension(ContentDispositionHeaderValue.Parse(model.File.ContentDisposition).FileName);
                if (fileExtension.Contains('.'))
                {
                    fileExtension = fileExtension.Trim('"').Trim('.');
                }
                else
                {
                    erros.Add((int)ErrosEnumeration.FormatoImagemInvalido);
                }
                if (!erros.Any())
                {
                    if (Enum.IsDefined(typeof(ExtensoesValidasEnumeration), fileExtension))
                    {
                        if (model.File.Length > 0)
                        {
                            categoria.ExtensaoImagem = fileExtension;
                            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Categoria", categoria.Nome + "." + categoria.ExtensaoImagem);
                            using FileStream fileStream = new FileStream(filePath, FileMode.Create);
                            await model.File.CopyToAsync(fileStream);
                            _categoriaDAO.EditarCategoria(categoria);
                        }
                        else
                        {
                            erros.Add((int)ErrosEnumeration.ImagemVazia);
                        }
                    }
                    else
                    {
                        erros.Add((int)ErrosEnumeration.FormatoImagemInvalido);
                    }
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }



        public ServiceResult EditarCategoria(CategoriaDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Nome))
            {
                throw new ArgumentNullException("Nome", "Parametro não pode ser nulo");
            }

            IList<int> erros = new List<int>();

            Categoria categoria = _categoriaDAO.GetCategoriaNome(model.Nome);

            if (categoria == null)
            {
                erros.Add((int)ErrosEnumeration.CategoriaNaoExiste);
            }
            else
            {
                if (!categoria.Nome.Equals(model.Nome))
                {
                    erros.Add((int)ErrosEnumeration.NomeCategoriaJaExiste);
                }
                else
                {
                    Categoria c = _mapper.Map<Categoria>(model);
                    _categoriaDAO.EditarCategoria(c);
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }



        public ServiceResult<IList<CategoriaDTO>> GetTodasCategorias()
        {
            IList<CategoriaDTO> categoriasDTO = null;

            IList<Categoria> categorias = _categoriaDAO.GetTodasCategorias();
            if (categorias != null)
            {
                string pathImagem = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Categoria");
                foreach (Categoria categoria in categorias)
                {
                    CategoriaDTO categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);
                    categoriaDTO.Url = new System.Security.Policy.Url(Path.Combine(pathImagem, categoria.Nome + "." + categoria.ExtensaoImagem));
                }
            }
            else
            {
                categoriasDTO = new List<CategoriaDTO>();
            }
            return new ServiceResult<IList<CategoriaDTO>> { Erros = null, Sucesso = true, Resultado =  categoriasDTO};
        }

        public ServiceResult<CategoriaDTO> GetCategoriaNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentNullException("Nome", "Parametro não pode ser nulo");
            }

            IList<int> erros = new List<int>();
            CategoriaDTO categoriaDTO = null;

            Categoria categoria = _categoriaDAO.GetCategoriaNome(nome);

            if (categoria == null)
            {
                erros.Add((int)ErrosEnumeration.CategoriaNaoExiste);
            }
            else
            {
                categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);
                string pathImagem = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Categoria");
                categoriaDTO.Url = new System.Security.Policy.Url(Path.Combine(pathImagem, categoria.Nome + "." + categoria.ExtensaoImagem));
            }

            return new ServiceResult<CategoriaDTO> { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any(), Resultado = categoriaDTO };
        }


        public async Task<ServiceResult> RemoverCategoria(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentNullException("Nome", "Parametro não pode ser nulo");
            }

            IList<int> erros = new List<int>();
            Categoria categoria = _categoriaDAO.GetCategoriaNome(nome);

            if (categoria == null)
            {
                erros.Add((int)ErrosEnumeration.CategoriaNaoExiste);
            }
            else
            {
                if (_categoriaDAO.CategoriaIsEmpty(nome))
                {
                    _categoriaDAO.RemoverCategoria(nome);

                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Categoria", nome + "." + categoria.ExtensaoImagem);
                    await Task.Factory.StartNew(() => File.Delete(filePath));
                }
            }
            return new ServiceResult { Erros = new ErrosDTO { Erros = erros }, Sucesso = !erros.Any() };
        }

    }
}
