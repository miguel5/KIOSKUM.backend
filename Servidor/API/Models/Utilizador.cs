﻿using System;
using System.Text;

namespace API.Models
{
    public abstract class Utilizador
    {
        public int IdUtilizador { get; }
        public string Nome { get; set; }
        public string PathImagem { get; set; }

        private static int id = 0;

        public Utilizador(string Nome, string PathImagem)
        {
            this.IdUtilizador = id++;
            this.Nome = Nome;
            this.PathImagem = PathImagem;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            Utilizador utilizador = (Utilizador)obj;
            return IdUtilizador == utilizador.IdUtilizador;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("- ID : " + IdUtilizador + "\n");
            sb.Append("- Nome : " + Nome + "\n");
            sb.Append("- Path Imagem: " + PathImagem + "\n");
            return sb.ToString();
        }
    }
}
