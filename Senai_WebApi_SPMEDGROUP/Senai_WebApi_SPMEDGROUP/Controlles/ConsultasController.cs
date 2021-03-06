﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senai_WebApi_SPMEDGROUP.Domains;
using Senai_WebApi_SPMEDGROUP.Interfaces;
using Senai_WebApi_SPMEDGROUP.Repositories;

namespace Senai_WebApi_SPMEDGROUP.Controlles
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultasController : ControllerBase
    {
        private IConsultaRepository ConsultaRepository { get; set; }

        public ConsultasController()
        {
            ConsultaRepository = new ConsultaRepository();
        }

        [Authorize]
        [HttpPut("situacao/{id}")]
        public IActionResult AtualizarConsulta(Consulta consulta, int id)
        {
            try
            {
                Consulta ConsultaBuscada = ConsultaRepository.BuscarPorId(id);

                if (ConsultaBuscada == null)
                {
                    return NotFound();
                }

                ConsultaBuscada.Descricao= consulta.Descricao;
                ConsultaRepository.AtualizarConsulta(ConsultaBuscada);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpGet]
        [Route("Usuario")]
        public IActionResult ListarPorUsuario()
        {
            try
            {
                int UsuarioId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault
                    (c => c.Type == JwtRegisteredClaimNames.Jti).Value);

                String TipoDeUsuarioId = (HttpContext.User.Claims.First
                    (c => c.Type == ClaimTypes.Role).Value);

                //Verifica se é do tipo paciente

                if (TipoDeUsuarioId.Equals("paciente") )
                {
                    using (SPMedGroupContext ctx = new SPMedGroupContext())
                    {
                        Pacientes pacienteLogado;
                        pacienteLogado = ctx.Paciente.FirstOrDefault(x => x.IdUsuario == UsuarioId);

                        return Ok(ConsultaRepository.ListarConsultasPaciente(pacienteLogado.Id));
                    }
                }
                else if (TipoDeUsuarioId.Equals("medico"))
                {
                    using (SPMedGroupContext ctx = new SPMedGroupContext())
                    {
                        Medico MedicoLogado;
                        MedicoLogado = ctx.Medico.FirstOrDefault(x => x.IdUsuario == UsuarioId);

                        return Ok(ConsultaRepository.ListarConsultasMedico(MedicoLogado.Id));
                    }
                }
                else if (TipoDeUsuarioId.Equals("administrador"))
                {
                    return Ok(ConsultaRepository.ListarTodasConsulta());
                }
                else
                {
                    return BadRequest();
                }
            }

            catch (Exception ex)
            {

                return BadRequest();
            }
        }

        /// <summary>
        /// Cadastra uma nova consulta no sistema
        /// </summary>
        /// <param name="DadosConsulta">Dados da consulta</param>
        /// <returns>Void tem retorno ?</returns>
        [Authorize]
        [HttpPost]
        public IActionResult Post(Consulta DadosConsulta)
        {
            try
            {
                ConsultaRepository.cadastrarConsulta(DadosConsulta);
                return Ok();
            }
            catch
            {

                return BadRequest();
            }
        }

        /// <summary>
        ///  Busca por todas as consultas do Paciente
        /// </summary>
        /// <param name="Id"> Id do paciente que as consultas serao buscadas</param>
        /// <returns>Retorna Uma lista de consultas de determinado Paciente</returns>
        [HttpGet("Paciente/{Id}")]
        [Authorize]
        public IActionResult getConsultas(int Id)
        {
            try
            {
                return Ok(ConsultaRepository.ListarConsultasPaciente(Id));
            }
            catch
            {

                return BadRequest();
            }
        }


        /// <summary>
        /// Busca por todas consultas de determinado Medico
        /// </summary>
        /// <param name="Id">Id do medico consultado</param>
        /// <returns>Retorna Uma lista de consultas de determinado Medico</returns>
        [HttpGet("Medico/{Id}")]
        [Authorize]
        public IActionResult getMedicos(int Id)
        {
            try
            {
                return Ok(ConsultaRepository.ListarConsultasMedico(Id));
            }
            catch
            {

                return BadRequest();
            }
        }
        /// <summary>
        /// Lista todas as Consultas Existentes
        /// </summary>
        /// <returns> Retorna uma lista com todas as consultas</returns>
        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            try
            {
                return Ok(ConsultaRepository.ListarTodasConsulta());
            }
            catch
            {

                return BadRequest();
            }
        }
    }
}