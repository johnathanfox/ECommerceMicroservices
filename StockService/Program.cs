// Adicione o MessageConsumer como um serviço hospedado
builder.Services.AddHostedService<MessageConsumer>();

var app = builder.Build();

// ...