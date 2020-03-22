using System.Collections.Generic;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace API.Controllers
{ 
    [ApiController]
    [Route("api/categoria")]
    public class CategoriaController : ControllerBase
    {
        private CategoriaDAO categoriaDAO = new CategoriaDAO();
        private readonly ILogger<CategoriaController> _logger;

        public CategoriaController(ILogger<CategoriaController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        [Route("todas")]
        public IList<Categoria> GetAllCategorias()
        {
            return null;
        }



        [HttpPost]
        public void AddCategoria(string Nome)
        {
            if (categoriaDAO.ContainsNome(Nome))
            {

            }
            else
            {
                categoriaDAO.AddNovaCategoria(Nome);

            }
        }
    }
}
