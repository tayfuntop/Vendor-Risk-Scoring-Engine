'use client';

import React from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Grid,
  FormControlLabel,
  Checkbox,
  Box,
  Typography,
  Chip,
  IconButton,
  useMediaQuery,
  useTheme,
} from '@mui/material';
import { Close as CloseIcon } from '@mui/icons-material';
import { useForm, Controller } from 'react-hook-form';
import { CreateVendorRequest } from '@/types/vendor';

interface VendorFormModalProps {
  open: boolean;
  onClose: () => void;
  onSubmit: (data: CreateVendorRequest) => Promise<void>;
}

interface FormData {
  name: string;
  financialHealth: number;
  slaUptime: number;
  majorIncidents: number;
  securityCerts: string;
  contractValid: boolean;
  privacyPolicyValid: boolean;
  pentestReportValid: boolean;
}

const VendorFormModal: React.FC<VendorFormModalProps> = ({ open, onClose, onSubmit }) => {
  const theme = useTheme();
  const fullScreen = useMediaQuery(theme.breakpoints.down('sm'));
  const {
    control,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<FormData>({
    defaultValues: {
      name: '',
      financialHealth: 50,
      slaUptime: 95,
      majorIncidents: 0,
      securityCerts: '',
      contractValid: true,
      privacyPolicyValid: true,
      pentestReportValid: true,
    },
  });

  const handleFormSubmit = async (data: FormData) => {
    const securityCertsArray = data.securityCerts
      .split(',')
      .map(cert => cert.trim())
      .filter(cert => cert.length > 0);

    const vendorData: CreateVendorRequest = {
      name: data.name,
      financialHealth: Number(data.financialHealth),
      slaUptime: Number(data.slaUptime),
      majorIncidents: Number(data.majorIncidents),
      securityCerts: securityCertsArray,
      documents: {
        contractValid: data.contractValid,
        privacyPolicyValid: data.privacyPolicyValid,
        pentestReportValid: data.pentestReportValid,
      },
    };

    await onSubmit(vendorData);
    reset();
    onClose();
  };

  const handleClose = () => {
    reset();
    onClose();
  };

  return (
    <Dialog
      open={open}
      onClose={handleClose}
      maxWidth="md"
      fullWidth
      fullScreen={fullScreen}
      sx={{
        '& .MuiDialog-paper': {
          m: { xs: 0, sm: 2 },
          maxHeight: { xs: '100%', sm: 'calc(100% - 64px)' }
        }
      }}
    >
      <DialogTitle>
        <Box display="flex" justifyContent="space-between" alignItems="center">
          <Typography variant="h6">Create New Vendor</Typography>
          <IconButton onClick={handleClose} size="small">
            <CloseIcon />
          </IconButton>
        </Box>
      </DialogTitle>
      <form onSubmit={handleSubmit(handleFormSubmit)}>
        <DialogContent dividers>
          <Grid container spacing={3}>
            <Grid size={{ xs: 12 }}>
              <Controller
                name="name"
                control={control}
                rules={{ required: 'Vendor name is required' }}
                render={({ field }) => (
                  <TextField
                    {...field}
                    label="Vendor Name"
                    fullWidth
                    error={!!errors.name}
                    helperText={errors.name?.message}
                  />
                )}
              />
            </Grid>

            <Grid size={{ xs: 12, md: 4 }}>
              <Controller
                name="financialHealth"
                control={control}
                rules={{
                  required: 'Financial health is required',
                  min: { value: 0, message: 'Minimum value is 0' },
                  max: { value: 100, message: 'Maximum value is 100' },
                }}
                render={({ field }) => (
                  <TextField
                    {...field}
                    label="Financial Health (0-100)"
                    type="number"
                    fullWidth
                    error={!!errors.financialHealth}
                    helperText={errors.financialHealth?.message}
                  />
                )}
              />
            </Grid>

            <Grid size={{ xs: 12, md: 4 }}>
              <Controller
                name="slaUptime"
                control={control}
                rules={{
                  required: 'SLA uptime is required',
                  min: { value: 0, message: 'Minimum value is 0' },
                  max: { value: 100, message: 'Maximum value is 100' },
                }}
                render={({ field }) => (
                  <TextField
                    {...field}
                    label="SLA Uptime (%)"
                    type="number"
                    fullWidth
                    error={!!errors.slaUptime}
                    helperText={errors.slaUptime?.message}
                  />
                )}
              />
            </Grid>

            <Grid size={{ xs: 12, md: 4 }}>
              <Controller
                name="majorIncidents"
                control={control}
                rules={{
                  required: 'Major incidents count is required',
                  min: { value: 0, message: 'Minimum value is 0' },
                }}
                render={({ field }) => (
                  <TextField
                    {...field}
                    label="Major Incidents (Last 12 months)"
                    type="number"
                    fullWidth
                    error={!!errors.majorIncidents}
                    helperText={errors.majorIncidents?.message}
                  />
                )}
              />
            </Grid>

            <Grid size={{ xs: 12 }}>
              <Controller
                name="securityCerts"
                control={control}
                render={({ field }) => (
                  <TextField
                    {...field}
                    label="Security Certifications"
                    fullWidth
                    placeholder="ISO27001, SOC2, PCI-DSS (comma separated)"
                    helperText="Enter certifications separated by commas"
                  />
                )}
              />
            </Grid>

            <Grid size={{ xs: 12 }}>
              <Typography variant="subtitle2" gutterBottom sx={{ mb: 1, fontWeight: 600 }}>
                Compliance Documents
              </Typography>
              <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
                <Controller
                  name="contractValid"
                  control={control}
                  render={({ field }) => (
                    <FormControlLabel
                      control={<Checkbox {...field} checked={field.value} />}
                      label="Contract Valid"
                    />
                  )}
                />
                <Controller
                  name="privacyPolicyValid"
                  control={control}
                  render={({ field }) => (
                    <FormControlLabel
                      control={<Checkbox {...field} checked={field.value} />}
                      label="Privacy Policy Valid"
                    />
                  )}
                />
                <Controller
                  name="pentestReportValid"
                  control={control}
                  render={({ field }) => (
                    <FormControlLabel
                      control={<Checkbox {...field} checked={field.value} />}
                      label="Penetration Test Report Valid"
                    />
                  )}
                />
              </Box>
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions sx={{ px: 3, py: 2 }}>
          <Button onClick={handleClose} disabled={isSubmitting}>
            Cancel
          </Button>
          <Button type="submit" variant="contained" disabled={isSubmitting}>
            {isSubmitting ? 'Creating...' : 'Create Vendor'}
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
};

export default VendorFormModal;
