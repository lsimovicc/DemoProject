﻿using AutoMapper;
using DemoProject.Dto;
using DemoProject.Application.Model;
using DemoProject.Application.Exceptions;
using DemoProject.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DemoProject.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController(ICategoryService categoryService, IMapper mapper) : ControllerBase
    {

        [HttpPost]
        public IActionResult AddCategory([FromBody] CategoryDto dto)
        {
            try
            {
                Category category = mapper.Map<Category>(dto);
                bool isCreated = categoryService.Create(category);
                if (!isCreated)
                {
                    return BadRequest(new { message = "Failed to create category." });
                }

                return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
            }
            catch (CategoryAlreadyExistsException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidParentCategoryException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(categoryService.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                Category? category = categoryService.GetById(id);

                return Ok(category);
            }
            catch (CategoryNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(Guid id, [FromBody] CategoryDto dto)
        {
            try
            {
                Category updatedCategory = mapper.Map<Category>(dto);
                updatedCategory.Id = id;
                Category? updated = categoryService.Update(updatedCategory);

                if (updated == null)
                {
                    return NotFound(new { message = $"Category with ID {id} not found." });
                }

                return Ok(updated);
            }
            catch (CategoryNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidParentCategoryException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(Guid id)
        {
            try
            {
                bool isDeleted = categoryService.Delete(id);
                if (!isDeleted)
                {
                    return NotFound(new { message = "Category not found or cannot be deleted due to subcategories." });
                }

                return Ok(new { message = "Category deleted successfully." });
            }
            catch (CategoryHasSubCategoriesException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (CategoryNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}