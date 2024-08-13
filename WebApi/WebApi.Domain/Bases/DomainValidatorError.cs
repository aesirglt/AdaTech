namespace WebApi.Domain.Bases;

using FunctionalConcepts.Errors;
using System;

public class DomainValidatorError : InvalidObjectError
{
    protected DomainValidatorError(
        string message,
        IDictionary<string, string[]> errors,
        Exception? exn) : base(message, exn)
    {
        Errors = errors;
    }

    public IDictionary<string, string[]> Errors { get; init; }

    public static DomainValidatorError New(
        string msg,
        IDictionary<string, string[]> errors,
        Exception? exn = null)
        => new(msg, errors, exn);

    public static implicit operator DomainValidatorError(
        (string Message, IDictionary<string, string[]> Errors) tuple)
        => new(tuple.Message, tuple.Errors, null);

    public static implicit operator DomainValidatorError(
        (string Message,
        IDictionary<string, string[]> Errors,
        Exception Exn) tuple)
        => new(tuple.Message, tuple.Errors, tuple.Exn);
}
