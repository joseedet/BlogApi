using System;
using FluentValidation;


namespace BlogApi.DTO;

/// <summary>
/// Validador para crear página Dto
/// </summary>
public class CrearPageDtoValidator : AbstractValidator<CrearPageDto>
{
    /// <summary>
    /// Valida la creación de pagina Dto.
    /// </summary>
    public CrearPageDtoValidator()
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
