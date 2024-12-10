namespace Common.Constants;

public enum DbEnums
{
    Equals,
    NotEquals,
    GreaterThan,
    LessThan,
    Contains
}

public enum QueryResultCode
{
    Ok = 200,
    Created = 201,
    NoContent = 204,
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    Conflict = 409,
    InternalServerError = 500,
}