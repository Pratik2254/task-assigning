﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TSAIdentity.Data;
using TSAIdentity.Models;

namespace TSAIdentity.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SkillsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SkillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Skills
        public async Task<IActionResult> Index()
        {
            var userEmail = HttpContext.User.Identity.Name;
            var organizationId = await _context.Organizations
                          .Where(o => o.OrganizationEmail == userEmail)
                          .Select(o => o.OrganizationId)
                          .FirstOrDefaultAsync();
            var applicationDbContext = _context.Skills.Include(s => s.Organization).Where(s=>s.OrganizationId==organizationId);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Skills/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Skills == null)
            {
                return NotFound();
            }

            var skill = await _context.Skills
                .Include(s => s.Organization)
                .FirstOrDefaultAsync(m => m.SkillId == id);
            if (skill == null)
            {
                return NotFound();
            }

            return View(skill);
        }

        // GET: Skills/Create
        public async Task<IActionResult> CreateAsync()
        {
            var userEmail = HttpContext.User.Identity?.Name;
            ViewData["OrganizationId"] = await _context.Organizations
                          .Where(o => o.OrganizationEmail == userEmail)
                          .Select(o => o.OrganizationId)
                          .FirstOrDefaultAsync();
            return View();
        }

        // POST: Skills/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SkillId,SkillName,OrganizationId")] Skill skill)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Skills.AnyAsync(s => s.SkillName == skill.SkillName && s.OrganizationId == skill.OrganizationId))
                {
                    ModelState.AddModelError("SkillName", "Similar skill already exists in the database.");
                    return View(skill);
                }
                skill.SkillId = Guid.NewGuid();
                _context.Add(skill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(skill);
        }

        // GET: Skills/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Skills == null)
            {
                return NotFound();
            }

            var skill = await _context.Skills.FindAsync(id);
            if (skill == null)
            {
                return NotFound();
            }
            return View(skill);
        }

        // POST: Skills/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("SkillId,SkillName,OrganizationId")] Skill skill)
        {
            if (id != skill.SkillId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (await _context.Skills.AnyAsync(s => s.SkillName == skill.SkillName && s.OrganizationId == skill.OrganizationId))
                {
                    ModelState.AddModelError("SkillName", "Similar skill already exists in the database.");
                    return View(skill);
                }
                try
                {
                    _context.Update(skill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SkillExists(skill.SkillId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(skill);
        }

        // GET: Skills/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Skills == null)
            {
                return NotFound();
            }

            var skill = await _context.Skills
                .Include(s => s.Organization)
                .FirstOrDefaultAsync(m => m.SkillId == id);
            if (skill == null)
            {
                return NotFound();
            }

            return View(skill);
        }

        // POST: Skills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Skills == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Skills'  is null.");
            }
            var skill = await _context.Skills.FindAsync(id);
            if (skill != null)
            {
                _context.Skills.Remove(skill);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SkillExists(Guid id)
        {
          return (_context.Skills?.Any(e => e.SkillId == id)).GetValueOrDefault();
        }
    }
}
