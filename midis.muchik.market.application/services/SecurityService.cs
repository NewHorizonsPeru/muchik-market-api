﻿using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly IJwtManger _jwtManger;
        private readonly IUserRepository _userRepository;

        public SecurityService(IMapper mapper, IJwtManger jwtManger, IUserRepository userRepository)
        {
            _mapper = mapper;
            _jwtManger = jwtManger;
            _userRepository = userRepository;
        }

        public GenericResponse<UserDto> SignIn(SignInRequestDto signInRequestDto)
        {
            var userExists = _userRepository.Find(w => w.Email.Equals(signInRequestDto.Email)).FirstOrDefault();
            if (userExists == null || !BcryptManager.Verify(signInRequestDto.Password, userExists.Password)) { 
                throw new MuchikException("Usuario y/o contraseña incorrecto, intente nuevamente."); 
            }
            var userDto = _mapper.Map<UserDto>(userExists);
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

            return new GenericResponse<UserDto>(userDto);
        }
    }
}
