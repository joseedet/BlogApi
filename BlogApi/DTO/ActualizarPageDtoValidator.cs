using System;
using FluentValidation;

namespace BlogApi.DTO;

/// <summary>
/// Validador para actualizar la página
/// </summary>
public class ActualizarPageDtoValidator : AbstractValidator<ActualizarPageDto>
{
    /// <summary>
    /// Validador para actualizar la página Dto
    /// </summary>
    public ActualizarPageDtoValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty()
            .WithMessage("El título es obligatorio")
            .MaximumLength(200)
            .WithMessage("El título no puede superar los 200 caracteres");
        RuleFor(x => x.Contenido).NotEmpty().WithMessage("El contenido es obligatorio");
        RuleFor(x => x.Publicado).NotNull().WithMessage("El estado de publicación es obligatorio");
    }
}
