using approval_workflow_backend.Guards;
using approval_workflow_backend.Infrastructure;
using approval_workflow_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace approval_workflow_backend.Services
{
    public class RequestService
    {
        private readonly AppDbContext _context;
        public RequestService(AppDbContext context)
        {
            _context = context;
        }

    }
}
