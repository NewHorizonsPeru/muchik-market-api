﻿using AutoMapper;
using Microsoft.Extensions.Configuration;
using midis.muchik.market.application.dto;
using midis.muchik.market.application.dto.security;
using midis.muchik.market.application.interfaces;
using midis.muchik.market.crosscutting.bcrypt;
using midis.muchik.market.crosscutting.exceptions;
using midis.muchik.market.crosscutting.interfaces;
using midis.muchik.market.crosscutting.models;
using midis.muchik.market.domain.entities;
using midis.muchik.market.domain.interfaces;

namespace midis.muchik.market.application.services
{
    public class SecurityService : ISecurityService
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IJwtManger _jwtManger;
        private readonly IMailManager _mailManager;
        private readonly IUserRepository _userRepository;

        public SecurityService(IConfiguration configuration, IMapper mapper, IJwtManger jwtManger, IMailManager mailManager, IUserRepository userRepository)
        {
            _configuration = configuration;
            _mapper = mapper;
            _jwtManger = jwtManger;
            _mailManager = mailManager;
            _userRepository = userRepository;
        }

        public GenericResponse<UserDto> SignIn(SignInRequestDto signInRequestDto)
        {
            var userExists = _userRepository.GetUserByEmail(signInRequestDto.Email);
            if (userExists == null || !BcryptManager.Verify(signInRequestDto.Password, userExists.Password)) { 
                throw new MuchikException("Usuario y/o contraseña incorrecto, intente nuevamente."); 
            }
            var userDto = _mapper.Map<UserDto>(userExists);
            userDto.Token = _jwtManger.GenerateToken(userDto.Id, userDto.Email, userDto.Role.Id);
            return new GenericResponse<UserDto>(userDto);
        }

        public GenericResponse<UserDto> SignUp(SignUpRequestDto signUpRequestDto)
        {
            var userExists = _userRepository.Find(w => w.Email.Equals(signUpRequestDto.Email)).FirstOrDefault();
            if(userExists != null) { throw new MuchikException("El correo ingresado ya existe, intente con otro."); }

            var userEntity = _mapper.Map<User>(signUpRequestDto);
            userEntity.Password = BcryptManager.Hash(userEntity.Password);
            _userRepository.Add(userEntity);
            
            _userRepository.Save();

            var userDto = _mapper.Map<UserDto>(userEntity);
            var pathMailingForgetPassword = _configuration.GetValue<string>("SendGridConfig:ForgetPasswordMailing");
            _mailManager.SendMail(pathMailingForgetPassword!, "srdelarosab@gmail.com;srdelarosab@icloud.com", new Dictionary<string, string>());
            return new GenericResponse<UserDto>(userDto);
        }

        public GenericResponse<string> ForgetPassword(ForgetPasswordDto forgetPasswordDto)
        {
            var userExists = _userRepository.Find(w => w.Email.Equals(forgetPasswordDto.Email)).FirstOrDefault();
            if (userExists is null) { throw new MuchikException("El correo ingresado no existe, intente con otro."); }

            var pathMailingForgetPassword = _configuration.GetValue<string>("SendGridConfig:ForgetPasswordMailing");
            var dictionaryMailingValues = new Dictionary<string, string>();
            dictionaryMailingValues.Add("[FULL_NAME]", userExists.Email);
            dictionaryMailingValues.Add("[FORGETPASSWORD_URL]", userExists.Email);
            _mailManager.SendMail(pathMailingForgetPassword!, userExists.Email, dictionaryMailingValues);
            return new GenericResponse<string>("Mail Sent!");
        }
    }
}
