# Dynamic Variables

MockHub supports dynamic content generation through a powerful template system. Use variables to create realistic, varied responses for your mock APIs. This section covers all available variables, template syntax, and advanced usage patterns.

## Overview

Dynamic variables enable:
- Realistic test data generation
- Request-dependent responses
- Conditional logic and transformations
- Array generation and iteration
- Date/time manipulation
- Random value generation

## Template Syntax

MockHub uses Scriban template syntax with double curly braces:

```
{{variable.name}}
{{function(parameters)}}
{{#if condition}}content{{/if}}
{{#repeat count}}item{{/repeat}}
```

## Faker Variables

Generate realistic fake data using the Faker library:

### Personal Information

```json
{
  "user": {
    "fullName": "{{faker.name.fullName}}",
    "firstName": "{{faker.name.firstName}}",
    "lastName": "{{faker.name.lastName}}",
    "jobTitle": "{{faker.name.jobTitle}}",
    "jobType": "{{faker.name.jobType}}"
  }
}
```

**Examples:**
- `{{faker.name.fullName}}` → "John Doe"
- `{{faker.name.firstName}}` → "Alice"
- `{{faker.name.lastName}}` → "Smith"
- `{{faker.name.jobTitle}}` → "Software Engineer"

### Internet & Communication

```json
{
  "contact": {
    "email": "{{faker.internet.email}}",
    "username": "{{faker.internet.userName}}",
    "url": "{{faker.internet.url}}",
    "ip": "{{faker.internet.ip}}",
    "ipv6": "{{faker.internet.ipv6}}",
    "mac": "{{faker.internet.mac}}",
    "color": "{{faker.internet.color}}",
    "domain": "{{faker.internet.domainName}}"
  }
}
```

**Examples:**
- `{{faker.internet.email}}` → "john.doe@example.com"
- `{{faker.internet.userName}}` → "johndoe_42"
- `{{faker.internet.url}}` → "https://example.com"
- `{{faker.internet.color}}` → "#3498db"

### Addresses & Locations

```json
{
  "address": {
    "city": "{{faker.address.city}}",
    "country": "{{faker.address.country}}",
    "streetAddress": "{{faker.address.streetAddress}}",
    "zipCode": "{{faker.address.zipCode}}",
    "latitude": "{{faker.address.latitude}}",
    "longitude": "{{faker.address.longitude}}"
  }
}
```

**Examples:**
- `{{faker.address.city}}` → "New York"
- `{{faker.address.country}}` → "United States"
- `{{faker.address.streetAddress}}` → "123 Main St"
- `{{faker.address.zipCode}}` → "10001"

### Commerce & Business

```json
{
  "product": {
    "name": "{{faker.commerce.productName}}",
    "price": "{{faker.commerce.price}}",
    "department": "{{faker.commerce.department}}",
    "adjective": "{{faker.commerce.productAdjective}}",
    "material": "{{faker.commerce.productMaterial}}"
  },
  "company": {
    "name": "{{faker.company.name}}",
    "catchPhrase": "{{faker.company.catchPhrase}}",
    "bs": "{{faker.company.bs}}"
  }
}
```

**Examples:**
- `{{faker.commerce.productName}}` → "Ergonomic Keyboard"
- `{{faker.commerce.price}}` → "299.99"
- `{{faker.company.name}}` → "Acme Corp"
- `{{faker.company.catchPhrase}}` → "Innovative Solutions"

### Date & Time

```json
{
  "timestamps": {
    "current": "{{now}}",
    "unix": "{{nowUnix}}",
    "recent": "{{faker.date.recent}}",
    "past": "{{faker.date.past}}",
    "future": "{{faker.date.future}}",
    "birthdate": "{{faker.date.birthdate}}"
  }
}
```

**Examples:**
- `{{now}}` → "2024-12-24T18:30:00Z"
- `{{nowUnix}}` → "1703442600"
- `{{faker.date.recent}}` → "2024-12-20"
- `{{faker.date.past}}` → "2023-06-15"

### Random Values

```json
{
  "random": {
    "uuid": "{{faker.random.uuid}}",
    "number": "{{faker.random.number}}",
    "boolean": "{{faker.random.boolean}}",
    "word": "{{faker.random.word}}",
    "sentence": "{{faker.lorem.sentence}}",
    "paragraph": "{{faker.lorem.paragraph}}"
  }
}
```

**Examples:**
- `{{faker.random.uuid}}` → "550e8400-e29b-41d4-a716-446655440000"
- `{{faker.random.number}}` → "42"
- `{{faker.random.boolean}}` → "true"
- `{{faker.lorem.sentence}}` → "Lorem ipsum dolor sit amet."

### Finance

```json
{
  "finance": {
    "account": "{{faker.finance.account}}",
    "amount": "{{faker.finance.amount}}",
    "currencyCode": "{{faker.finance.currencyCode}}",
    "creditCard": "{{faker.finance.creditCardNumber}}",
    "iban": "{{faker.finance.iban}}"
  }
}
```

**Examples:**
- `{{faker.finance.account}}` → "12345678"
- `{{faker.finance.amount}}` → "1,234.56"
- `{{faker.finance.currencyCode}}` → "USD"
- `{{faker.finance.iban}}` → "GB29 NWBK 6016 1331 9268 19"

## Request Variables

Access incoming request data in your responses:

### URL Parameters

```json
{
  "pathParams": {
    "userId": "{{request.params.id}}",
    "postId": "{{request.params.postId}}"
  },
  "queryParams": {
    "page": "{{request.query.page}}",
    "limit": "{{request.query.limit}}",
    "filter": "{{request.query.status}}"
  }
}
```

**Examples:**
- Request: `GET /api/users/123/posts/456?page=2&limit=10`
- `{{request.params.id}}` → "123"
- `{{request.params.postId}}` → "456"
- `{{request.query.page}}` → "2"
- `{{request.query.limit}}` → "10"

### Request Headers

```json
{
  "headers": {
    "userAgent": "{{request.headers.User-Agent}}",
    "authorization": "{{request.headers.Authorization}}",
    "contentType": "{{request.headers.Content-Type}}",
    "custom": "{{request.headers.X-Custom-Header}}"
  }
}
```

**Examples:**
- `{{request.headers.Authorization}}` → "Bearer eyJhbGciOiJIUzI1NiIs..."
- `{{request.headers.User-Agent}}` → "Mozilla/5.0 ..."

### Request Body

Access JSON/XML request body fields:

```json
{
  "requestData": {
    "name": "{{request.body.name}}",
    "email": "{{request.body.email}}",
    "active": "{{request.body.active}}"
  }
}
```

**Example Request Body:**
```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "active": true
}
```

**Response using body data:**
```json
{
  "message": "User {{request.body.name}} created successfully",
  "email": "{{request.body.email}}",
  "status": "{{#if request.body.active}}active{{else}}inactive{{/if}}"
}
```

### Request Metadata

```json
{
  "request": {
    "id": "{{request.id}}",
    "method": "{{request.method}}",
    "path": "{{request.path}}"
  }
}
```

**Examples:**
- `{{request.id}}` → "req_abc123"
- `{{request.method}}` → "GET"
- `{{request.path}}` → "/api/users"

## Template Logic

### Conditional Statements

Use if/else logic for dynamic responses:

```json
{
  "status": "{{#if request.query.success}}success{{else}}error{{/if}}",
  "message": "{{#if request.params.id}}User found{{else}}User not found{{/if}}",
  "data": "{{#if request.body.active}}{{faker.lorem.paragraph}}{{else}}Inactive{{/if}}"
}
```

### Loops and Arrays

Generate dynamic arrays with repeat:

```json
{
  "users": [
    {{#repeat request.query.count | default: 3}}
    {
      "id": {{@@index + 1}},
      "name": "{{faker.name.fullName}}",
      "email": "{{faker.internet.email}}"
    }{{#unless @@last}},{{/unless}}
    {{/repeat}}
  ]
}
```

**Loop Variables:**
- `@@index`: Current iteration (0-based)
- `@@index + 1`: 1-based index
- `@@last`: Boolean indicating last iteration

### Advanced Loops

```json
{
  "products": [
    {{#repeat 5}}
    {
      "id": {{@@index + 1}},
      "name": "{{faker.commerce.productName}}",
      "price": {{faker.commerce.price | math.round 2}},
      "category": "{{faker.commerce.department}}",
      "inStock": {{faker.random.boolean}}
    }{{#unless @@last}},{{/unless}}
    {{/repeat}}
  ]
}
```

## Filters and Functions

### Math Operations

```json
{
  "calculations": {
    "price": {{faker.commerce.price | math.round 2}},
    "discounted": {{faker.commerce.price | math.multiply 0.8 | math.round 2}},
    "id": {{request.query.id | math.add 1000}}
  }
}
```

### String Operations

```json
{
  "text": {
    "uppercase": "{{faker.name.fullName | string.upcase}}",
    "lowercase": "{{faker.name.fullName | string.downcase}}",
    "capitalized": "{{faker.name.firstName | string.capitalize}}",
    "length": {{faker.lorem.sentence | string.size}}
  }
}
```

### Default Values

```json
{
  "pagination": {
    "page": {{request.query.page | default: 1}},
    "limit": {{request.query.limit | default: 10}},
    "sort": "{{request.query.sort | default: 'name'}}"
  }
}
```

## Advanced Examples

### User Profile API

```json
{
  "id": "{{faker.random.uuid}}",
  "name": "{{faker.name.fullName}}",
  "email": "{{faker.internet.email}}",
  "profile": {
    "avatar": "{{faker.image.avatar}}",
    "bio": "{{faker.lorem.paragraph}}",
    "location": "{{faker.address.city}}, {{faker.address.country}}",
    "website": "{{faker.internet.url}}"
  },
  "stats": {
    "posts": {{faker.random.number}},
    "followers": {{faker.random.number}},
    "following": {{faker.random.number}}
  },
  "createdAt": "{{faker.date.past}}",
  "isActive": {{faker.random.boolean}}
}
```

### E-commerce Product API

```json
{
  "product": {
    "id": "{{faker.random.uuid}}",
    "name": "{{faker.commerce.productName}}",
    "description": "{{faker.lorem.paragraph}}",
    "price": {{faker.commerce.price | math.round 2}},
    "category": "{{faker.commerce.department}}",
    "tags": [
      "{{faker.commerce.productAdjective}}",
      "{{faker.commerce.productMaterial}}",
      "{{faker.commerce.productName | string.split | array.first}}"
    ],
    "images": [
      "{{faker.image.url 400 400}}",
      "{{faker.image.url 400 400}}",
      "{{faker.image.url 400 400}}"
    ],
    "inventory": {
      "quantity": {{faker.random.number}},
      "inStock": true
    },
    "reviews": [
      {{#repeat faker.random.number 1 5}}
      {
        "id": {{@@index + 1}},
        "user": "{{faker.name.fullName}}",
        "rating": {{faker.random.number 1 5}},
        "comment": "{{faker.lorem.sentence}}",
        "date": "{{faker.date.recent}}"
      }{{#unless @@last}},{{/unless}}
      {{/repeat}}
    ]
  }
}
```

### Paginated List API

```json
{
  "data": [
    {{#repeat request.query.limit | default: 10}}
    {
      "id": {{@@index + (request.query.page | default: 1 | math.multiply (request.query.limit | default: 10)) - (request.query.limit | default: 10) + 1}},
      "title": "{{faker.lorem.sentence | string.rtruncate 50}}",
      "content": "{{faker.lorem.paragraph}}",
      "author": {
        "name": "{{faker.name.fullName}}",
        "email": "{{faker.internet.email}}"
      },
      "publishedAt": "{{faker.date.recent}}",
      "tags": [
        "{{faker.random.word}}",
        "{{faker.random.word}}"
      ]
    }{{#unless @@last}},{{/unless}}
    {{/repeat}}
  ],
  "pagination": {
    "page": {{request.query.page | default: 1}},
    "limit": {{request.query.limit | default: 10}},
    "total": 100,
    "totalPages": {{100 | math.divide (request.query.limit | default: 10) | math.ceil}}
  },
  "meta": {
    "requestId": "{{request.id}}",
    "timestamp": "{{now}}"
  }
}
```

### Error Response with Context

```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Invalid request parameters",
    "details": [
      {
        "field": "email",
        "message": "Invalid email format",
        "value": "{{request.body.email}}"
      }
    ],
    "request": {
      "method": "{{request.method}}",
      "path": "{{request.path}}",
      "userAgent": "{{request.headers.User-Agent}}"
    },
    "timestamp": "{{now}}",
    "requestId": "{{request.id}}"
  }
}
```

## Best Practices

### Performance
- Use simple expressions for frequently called endpoints
- Cache expensive operations when possible
- Limit array sizes in responses
- Avoid deeply nested template logic

### Maintainability
- Use meaningful variable names
- Document complex template logic
- Group related variables together
- Test templates with various inputs

### Security
- Don't expose sensitive data through variables
- Validate request parameters before using in responses
- Use appropriate data types
- Sanitize user inputs

### Testing
- Test templates with edge cases
- Verify all conditional paths
- Check array generation with different sizes
- Validate date/time formats

## Variable Reference

### Complete Faker Reference

For a complete list of available Faker methods, refer to the Faker.js documentation. MockHub supports most Faker.js methods through the `faker.` prefix.

### Custom Variables

*Future feature - define custom variables and functions*

## Troubleshooting

### Variables Not Rendering

**Problem**: Variables show as `{{variable.name}}` instead of values

**Solutions:**
- Check syntax (double curly braces)
- Verify variable names are correct
- Ensure proper JSON structure
- Check for syntax errors in templates

### Request Variables Empty

**Problem**: `request.*` variables return empty values

**Solutions:**
- Verify request contains the expected parameters
- Check parameter names match exactly
- Ensure correct case sensitivity
- Test with simple request variables first

### Template Errors

**Problem**: Template compilation fails

**Solutions:**
- Check for syntax errors
- Verify bracket matching
- Use proper Scriban syntax
- Test with simpler templates first

### Performance Issues

**Problem**: Template rendering is slow

**Solutions:**
- Simplify complex expressions
- Reduce array sizes
- Minimize conditional logic
- Cache static content
